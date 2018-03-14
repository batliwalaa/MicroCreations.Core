using MicroCreations.Batch.Common.Context;

namespace MicroCreations.Batch.Common.DependencyGraph
{
    public class DefaultDependencyOperationPredicate : IDependencyOperationPredicate
    {
        public bool AllowExecute(BatchExecutionContext batchExecutionContext)
        {
            return true;
        }

        public bool SupportsOperation(string currentOperationName)
        {
            return true;
        }
    }
}
