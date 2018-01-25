using System.Collections.Generic;

namespace MicroCreations.Batch.Common.DependencyGraph
{
    public interface IDependencyGraph
    {
        IEnumerable<DependencyNode> ResolveChildren(string key);
    }
}
