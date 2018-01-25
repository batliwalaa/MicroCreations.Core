using MicroCreations.Batch.Common.DependencyGraph;
using System.Collections.Generic;

namespace MicroCreations.Batch.DependencyGraph
{
    public class DefaultDependencyGraph : IDependencyGraph
    {
        private readonly IReadOnlyDictionary<string, DependencyNode> _dependencyNodeCollection;

        public DefaultDependencyGraph(IDependencyGraphBuilder dependencyGraphBuilder)
        {
            _dependencyNodeCollection = dependencyGraphBuilder.DependencyNodes;
        }

        public IEnumerable<DependencyNode> ResolveChildren(string key)
        {
            if (_dependencyNodeCollection == null) return null;

            var invariantKey = key.ToLowerInvariant();

            return _dependencyNodeCollection.ContainsKey(invariantKey) ? _dependencyNodeCollection[invariantKey].Children : null;
        }
    }
}