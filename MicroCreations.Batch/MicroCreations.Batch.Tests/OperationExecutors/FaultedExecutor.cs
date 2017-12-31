using System;
using System.Threading.Tasks;
using MicroCreations.Batch.Domain;
using MicroCreations.Batch.Domain.Interfaces;

namespace MicroCreations.Batch.Tests.OperationExecutors
{
    public class FaultedExecutor : IOperationExecutor
    {
        public FaultedExecutor()
        {
            SupportedOperationName = "FaultedExecutor";
        }

        public string SupportedOperationName { get; }

        public async Task<OperationResult> Execute(BatchExecutionContext context)
        {
            await Task.Factory.StartNew(() => { });

            throw new NotImplementedException();
        }
    }
}
