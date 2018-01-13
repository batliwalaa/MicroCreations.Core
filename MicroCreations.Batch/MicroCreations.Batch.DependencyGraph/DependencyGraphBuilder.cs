using System.Collections.Generic;

namespace MicroCreations.Batch.DependencyGraph
{
    public class DependencyGraphBuilder : IDependencyGraphBuilder
    {
        private readonly Dictionary<string, DependencyNode> _dependencyNodeCollection;

        public DependencyGraphBuilder()
        {
            _dependencyNodeCollection = new Dictionary<string, DependencyNode>();
        }

        public IReadOnlyDictionary<string, DependencyNode> DependencyNodes => _dependencyNodeCollection;

        public void Add(DependencyNode dependencyNode)
        {
            _dependencyNodeCollection.Add(dependencyNode.Key.ToLowerInvariant(), dependencyNode);
        }
    }
}