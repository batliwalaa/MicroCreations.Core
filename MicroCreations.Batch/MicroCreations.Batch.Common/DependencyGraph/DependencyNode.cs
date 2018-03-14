using System.Collections.Generic;

namespace MicroCreations.Batch.Common.DependencyGraph
{
    public class DependencyNode
    {
        public DependencyNode() 
            : this(new DefaultDependencyOperationPredicate())
        {
        }

        public DependencyNode(IDependencyOperationPredicate dependencyOperationPredicate)
        {
            DependencyOperationPredicate = dependencyOperationPredicate;
        }

        public string Key { get; set; }

        public IEnumerable<DependencyNode> Children { get; set; }

        public IDependencyOperationPredicate DependencyOperationPredicate { get; }
    }
}
