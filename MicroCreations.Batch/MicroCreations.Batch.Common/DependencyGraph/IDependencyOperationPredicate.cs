using MicroCreations.Batch.Common.Context;

namespace MicroCreations.Batch.Common.DependencyGraph
{
    public interface IDependencyOperationPredicate
    {
        bool SupportsOperation(string currentOperationName);

        bool AllowExecute(BatchExecutionContext batchExecutionContext);
    }
}
