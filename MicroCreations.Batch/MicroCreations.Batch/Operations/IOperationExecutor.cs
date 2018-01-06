using System.Threading.Tasks;
using MicroCreations.Batch.Context;

namespace MicroCreations.Batch.Operations
{
    public interface IOperationExecutor
    {
        string SupportedOperationName { get; }

        Task<OperationResult> Execute(BatchExecutionContext context);
    }
}
