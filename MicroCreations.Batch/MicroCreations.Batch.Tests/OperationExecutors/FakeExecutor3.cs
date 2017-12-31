using System.Linq;
using System.Threading.Tasks;
using MicroCreations.Batch.Domain;
using MicroCreations.Batch.Domain.Interfaces;

namespace MicroCreations.Batch.Tests.OperationExecutors
{
    public class FakeExecutor3 : IOperationExecutor
    {
        public FakeExecutor3()
        {
            SupportedOperationName = "Operation 3";
        }
        public string SupportedOperationName { get; }

        public async Task<OperationResult> Execute(BatchExecutionContext context)
        {
            return await Task.Factory.StartNew(() => new OperationResult { Value = context.Arguments.First().Value });
        }
    }
}
