using System.Collections.Generic;

namespace MicroCreations.Batch.DependencyGraph
{
    public interface IDependencyGraph
    {
        IEnumerable<DependencyNode> ResolveChildren(string key);
    }
}
