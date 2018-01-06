using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MicroCreations.Batch.Operations;

namespace MicroCreations.Batch.Processors
{
    internal class SerialProcessor : BaseProcessor
    {
        public override ProcessingType ProcessingType => ProcessingType.Serial;

        public override async Task<IEnumerable<OperationResult>> ProcessAsync(ProcessRequest processRequest)
        {
            var results = new List<OperationResult>();

            foreach (var executor in processRequest.Executors)
            {
                try
                {
                    var result = await executor.Execute(GetBatchExecutionContext(processRequest.Request.Arguments, processRequest.ApplicationContext, results, CancellationToken.None));

                    results.Add(result);
                }
                catch (Exception ex)
                {
                    results.Add(GetFaultedOperationResult(executor.SupportedOperationName, ex));
                }

                if (processRequest.Request.FaultCancellationOption == FaultCancellationOption.Cancel && results.Any(x => x.IsFaulted))
                {
                    break;
                }
            }

            return results;
        }
    }
}
