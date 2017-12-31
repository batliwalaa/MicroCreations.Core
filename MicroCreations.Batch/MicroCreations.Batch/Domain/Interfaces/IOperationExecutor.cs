using System.Threading.Tasks;

namespace MicroCreations.Batch.Domain.Interfaces
{
    public interface IOperationExecutor
    {
        string SupportedOperationName { get; }

        Task<OperationResult> Execute(BatchExecutionContext context);
    }
}
