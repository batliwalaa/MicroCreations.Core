using MicroCreations.Batch.Common.Context;
using MicroCreations.Batch.Common.Operations;

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
