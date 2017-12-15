using MicroCreations.Core.OperationAggregation.Domain.Interfaces;
using System.Collections.Generic;
using MicroCreations.Core.OperationAggregation.Domain;
using MicroCreations.Core.OperationAggregation.Enums;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Web;
using System;
using System.Configuration;

namespace MicroCreations.Core.OperationAggregation
{
    public class OperationAggregator : IOperationAggregator
    {
        private readonly IEnumerable<IOperationExecutor> _executors;

        public OperationAggregator(IEnumerable<IOperationExecutor> executors)
        {
            _executors = executors;
        }

        public BatchOperationResponse Execute(BatchOperationRequest request)
        {
            var adjacentGroups = GetAdjacentGroups(request.Operations);
            var results = new List<OperationResult>();

            foreach(var kvp in adjacentGroups)
            {
                if(kvp.Key == ProcessingType.Serial)
                {
                    results.AddRange(ProcessSerial(request.FaultCancellationOption, request.Arguments, GetExecutors(kvp.Value)));
                }
                else
                {
                    results.AddRange(ProcessParallel(request.FaultCancellationOption, request.Arguments, GetExecutors(kvp.Value)));
                }

                if(request.FaultCancellationOption == FaultCancellationOption.Cancel && results.Any(x => x.IsFaulted))
                {
                    break;
                }
            }

            return new BatchOperationResponse { Results = results };
        }

        private IEnumerable<OperationResult> ProcessParallel(
            FaultCancellationOption faultCancellationOption,
            IEnumerable<OperationArgument> arguments,
            IEnumerable<IOperationExecutor> executors)
        {
            var results = new List<OperationResult>();
            var cancellationTokenSource = new CancellationTokenSource();
            var mainThreadHttpContext = HttpContext.Current;
            var parallelOptions = GetParallelOptions(cancellationTokenSource);

            try
            {
                Parallel.ForEach(executors, parallelOptions, (executor, state) =>
                {
                    try
                    {
                        HttpContext.Current = mainThreadHttpContext;

                        if (!parallelOptions.CancellationToken.IsCancellationRequested)
                        {
                            executor.Execute(GetOperationExecutionContext(arguments, results, parallelOptions.CancellationToken));
                        }
                    }
                    catch (Exception ex)
                    {
                        if (faultCancellationOption == FaultCancellationOption.Cancel)
                        {
                            cancellationTokenSource.Cancel();
                        }
                        results.Add(GetFaultedOperationResult(executor.SupportedOperationName, ex));
                    }
                });
            }
            catch { } // do nothing, as each task in parallel handles exception, and exception is returned back to client.
            finally
            {
                cancellationTokenSource.Dispose();
            }
            return results;
        }

        private IEnumerable<OperationResult> ProcessSerial(
            FaultCancellationOption faultCancellationOption,
            IEnumerable<OperationArgument> arguments,
            IEnumerable<IOperationExecutor> executors)
        {
            var results = new List<OperationResult>();

            foreach(var executor in executors)
            {
                try
                {
                    results.Add(executor.Execute(GetOperationExecutionContext(arguments, results, CancellationToken.None)));
                }
                catch (Exception ex)
                {
                    results.Add(GetFaultedOperationResult(executor.SupportedOperationName, ex));
                }

                if(faultCancellationOption == FaultCancellationOption.Cancel && results.Any(x => x.IsFaulted))
                {
                    break;
                }
            }

            return results;
        }

        private IDictionary<ProcessingType, IEnumerable<Operation>> GetAdjacentGroups(IEnumerable<Operation> operations)
        {
            var dictionary = new Dictionary<ProcessingType, IEnumerable<Operation>>();
            var currentKey = default(ProcessingType);
            var list = new List<Operation>();

            foreach(var operation in operations)
            {
                if(operation.ProcessingType != currentKey)
                {
                    if (list.Any())
                    {
                        dictionary.Add(currentKey, list);
                        list = null;
                    }

                    currentKey = operation.ProcessingType;
                }

                list.Add(operation);
            }

            if (list.Any())
            {
                dictionary.Add(currentKey, list);

            }

            return dictionary;
        }

        private IEnumerable<IOperationExecutor> GetExecutors(IEnumerable<Operation> operations)
        {
            var operationNames = operations.Select(x => x.OperationName);

            return _executors.Where(x => operationNames.Contains(x.SupportedOperationName));
        }

        private static OperationResult GetFaultedOperationResult(string operationName, Exception exception)
        {
            return new OperationResult
            {
                Exception = exception,
                IsFaulted = true,
                OperationName = operationName
            };
        }

        private static OperationExecutionContext GetOperationExecutionContext(
            IEnumerable<OperationArgument> arguments,
            IEnumerable<OperationResult> results,
            CancellationToken cancellationToken)
        {
            return new OperationExecutionContext
            {
                Arguments = arguments,
                CancellationToken = cancellationToken,
                Results = results
            };
        }

        private static int? ToNullableInt(string value)
        {
            var result = default(int?);
            int intValue;

            if(int.TryParse(value, out intValue))
            {
                result = intValue;
            }

            return result;
        }

        private static ParallelOptions GetParallelOptions(CancellationTokenSource cts)
        {
            var parallelOptions = new ParallelOptions
            {
                CancellationToken = cts.Token
            };

            var maxDegreeofParallelism = ToNullableInt(ConfigurationManager.AppSettings["MicroCreations_Core_OperationsAggregation_MaxDegreeOfParallelism"]);
            if (maxDegreeofParallelism.HasValue)
            {
                parallelOptions.MaxDegreeOfParallelism = maxDegreeofParallelism.Value;
            }

            return parallelOptions;
        }
    }
}
