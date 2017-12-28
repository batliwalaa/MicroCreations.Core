using MicroCreations.Core.OperationAggregation.Domain.Interfaces;
using System.Collections.Generic;
using MicroCreations.Core.OperationAggregation.Domain;
using MicroCreations.Core.OperationAggregation.Enums;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System;
using System.Configuration;

namespace MicroCreations.Core.OperationAggregation
{
    public class OperationAggregator : IOperationAggregator
    {
        private readonly IEnumerable<IOperationExecutor> _executors;
        private readonly IContextBuilder _contextBuilder;

        public OperationAggregator(
            IEnumerable<IOperationExecutor> executors,
            IContextBuilder contextBuilder)
        {
            _executors = executors;
            _contextBuilder = contextBuilder;
        }

        public BatchOperationResponse Execute(BatchOperationRequest request)
        {
            return ExecuteBatch(request);
        }

        public async Task<BatchOperationResponse> ExecuteAsync(BatchOperationRequest request)
        {
            return await Task.Factory.StartNew(() => ExecuteBatch(request));
        }

        private BatchOperationResponse ExecuteBatch(BatchOperationRequest request)
        {
            var adjacentGroups = GetAdjacentGroups(request.Operations);
            var results = new List<OperationResult>();
            var context = _contextBuilder.GetContext();

            foreach (var kvp in adjacentGroups)
            {
                if (kvp.Key == ProcessingType.Serial)
                {
                    results.AddRange(ProcessSerial(request, context, GetExecutors(kvp.Value)));
                }
                else
                {
                    results.AddRange(ProcessParallel(request, context, GetExecutors(kvp.Value)));
                }

                if (request.FaultCancellationOption == FaultCancellationOption.Cancel && results.Any(x => x.IsFaulted))
                {
                    break;
                }
            }

            return new BatchOperationResponse { Results = results };
        }

        private static IEnumerable<OperationResult> ProcessParallel(
            BatchOperationRequest request,
            IContext context,
            IEnumerable<IOperationExecutor> executors)
        {
            var results = new List<OperationResult>();
            var cancellationTokenSource = new CancellationTokenSource();
            var parallelOptions = GetParallelOptions(cancellationTokenSource);

            try
            {
                Parallel.ForEach(executors, parallelOptions, (executor, state) =>
                {
                    try
                    {
                        if (!parallelOptions.CancellationToken.IsCancellationRequested)
                        {
                            results.Add(executor.Execute(GetOperationExecutionContext(request.Arguments, context, results, parallelOptions.CancellationToken)));
                        }
                    }
                    catch (Exception ex)
                    {
                        if (request.FaultCancellationOption == FaultCancellationOption.Cancel)
                        {
                            // ReSharper disable once AccessToDisposedClosure
                            cancellationTokenSource.Cancel();
                        }
                        results.Add(GetFaultedOperationResult(executor.SupportedOperationName, ex));
                    }
                });
            }
            // ReSharper disable once EmptyGeneralCatchClause
            catch { } // do nothing, as each task in parallel handles exception, and exception is returned back to client.
            finally
            {
                cancellationTokenSource.Dispose();
            }
            return results;
        }

        private static IEnumerable<OperationResult> ProcessSerial(
            BatchOperationRequest request,
            IContext context,
            IEnumerable<IOperationExecutor> executors)
        {
            var results = new List<OperationResult>();

            foreach(var executor in executors)
            {
                try
                {
                    results.Add(executor.Execute(GetOperationExecutionContext(request.Arguments, context, results, CancellationToken.None)));
                }
                catch (Exception ex)
                {
                    results.Add(GetFaultedOperationResult(executor.SupportedOperationName, ex));
                }

                if(request.FaultCancellationOption == FaultCancellationOption.Cancel && results.Any(x => x.IsFaulted))
                {
                    break;
                }
            }

            return results;
        }

        private static IDictionary<ProcessingType, IEnumerable<Operation>> GetAdjacentGroups(IEnumerable<Operation> operations)
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
                        list = new List<Operation>();
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
            IContext context,
            IEnumerable<OperationResult> results,
            CancellationToken cancellationToken)
        {
            return new OperationExecutionContext(arguments, results, cancellationToken, context);
        }

        private static int? ToNullableInt(string value)
        {
            var result = default(int?);

            if(int.TryParse(value, out var intValue))
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

        private IEnumerable<IOperationExecutor> GetExecutors(IEnumerable<Operation> operations)
        {
            var operationNames = operations.Select(x => x.OperationName);

            return _executors.Where(x => operationNames.Contains(x.SupportedOperationName));
        }
    }
}
