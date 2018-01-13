using System.Threading.Tasks;
using MicroCreations.Batch.Common.Context;

namespace MicroCreations.Batch.Common.Operations
{
    public interface IOperationExecutor
    {
        string SupportedOperationName { get; }

        Task<OperationResult> Execute(BatchExecutionContext context);
    }
}
