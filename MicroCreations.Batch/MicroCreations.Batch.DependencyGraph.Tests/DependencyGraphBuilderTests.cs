using MicroCreations.Batch.Common.DependencyGraph;
using NUnit.Framework;
using FluentAssertions;

namespace MicroCreations.Batch.DependencyGraph.Tests
{
    [TestFixture]
    public class DependencyGraphBuilderTests
    {
        private IDependencyGraphBuilder _dependencyGraphBuilder;

        [SetUp]
        public void Setup()
        {
            _dependencyGraphBuilder = new DependencyGraphBuilder();
        }

        [Test]
        public void When_Add_Invoked_With_DependencyNode_Expect_DependencyNodes_Count_Equals_One()
        {
            _dependencyGraphBuilder.Add(new DependencyNode { Key = "Operation 1" });

            _dependencyGraphBuilder.DependencyNodes.Count.ShouldBeEquivalentTo(1);
        }
    }
}
