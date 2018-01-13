using System.Collections.Generic;

namespace MicroCreations.Batch.DependencyGraph
{
    public interface IDependencyGraphBuilder
    {
        void Add(DependencyNode dependencyNode);

        IReadOnlyDictionary<string, DependencyNode> DependencyNodes { get; }
    }
}
