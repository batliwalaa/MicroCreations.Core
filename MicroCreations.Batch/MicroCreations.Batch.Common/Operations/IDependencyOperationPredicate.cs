using MicroCreations.Batch.Common.Context;

namespace MicroCreations.Batch.Common.Operations
{
    public interface IDependencyOperationPredicate
    {
        string OperationName { get; set; }

        bool Predicate(BatchExecutionContext batchExecutionContext);
    }
}
