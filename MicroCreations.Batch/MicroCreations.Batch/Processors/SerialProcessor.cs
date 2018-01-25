using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MicroCreations.Batch.Common;
using MicroCreations.Batch.Common.Operations;

namespace MicroCreations.Batch.Processors
{
    internal class SerialProcessor : BaseProcessor
    {
        public SerialProcessor(IEnumerable<IOperationExecutor> executors)
            : base(executors)
        {
        }

        public override ProcessorType ProcessorType => ProcessorType.Serial;

        public override async Task<IEnumerable<OperationResult>> ProcessAsync(ProcessRequest processRequest)
        {
            var results = new List<OperationResult>(processRequest.Results);
            var executors = GetExecutors(processRequest.Operations);

            if (executors.Any())
            {
                foreach (var executor in executors)
                {
                    try
                    {
                        var result = await executor.Execute(GetBatchExecutionContext(processRequest.Arguments, processRequest.ApplicationContext, results, CancellationToken.None));

                        results.Add(result);
                    }
                    catch (Exception ex)
                    {
                        results.Add(GetFaultedOperationResult(executor.SupportedOperationName, ex));
                    }

                    if (processRequest.FaultCancellationOption == FaultCancellationOption.Cancel && results.Any(x => x.IsFaulted))
                    {
                        break;
                    }
                }
            }

            return results;
        }
    }
}
