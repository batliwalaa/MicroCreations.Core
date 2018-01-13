using System.Collections.Generic;;

namespace MicroCreations.Batch.DependencyGraph
{
    public class DependencyNode
    {
        public string Key { get; set; }

        public IEnumerable<DependencyNode> Children { get; set; }
    }
}
