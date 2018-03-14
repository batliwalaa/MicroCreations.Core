using MicroCreations.Batch.Common.DependencyGraph;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;

namespace MicroCreations.Batch.DependencyGraph.Tests
{
    [TestFixture]
    public class DefaultDependencyGraphTests
    {
        private Mock<IDependencyGraphBuilder> _dependencyGraphBuilderMock;
        private IDependencyGraph _dependencyGraph;

        [SetUp]
        public void Setup()
        {
            _dependencyGraphBuilderMock = new Mock<IDependencyGraphBuilder>();
        }

        [Test]
        public void When_ResolveChildren_Invoked_Expect_Correct_Behaviour()
        {
            _dependencyGraphBuilderMock.SetupGet(x => x.DependencyNodes).Returns(GetDependencyNodes()).Verifiable();
            _dependencyGraph = new DefaultDependencyGraph(_dependencyGraphBuilderMock.Object);

            var dependencyNodes = _dependencyGraph.ResolveChildren("Operation 1");

            dependencyNodes.Should().BeOfType<DependencyNode[]>();
            dependencyNodes.Count().ShouldBeEquivalentTo(1);
            _dependencyGraphBuilderMock.VerifyGet(x => x.DependencyNodes, Times.Once);
        }

        private static IReadOnlyDictionary<string, DependencyNode> GetDependencyNodes()
        {
            var dictionary = new Dictionary<string, DependencyNode>();

            dictionary.Add("Operation 1".ToLowerInvariant(), new DependencyNode { Key = "Operation 1", Children = new[] { new DependencyNode() } });

            return dictionary;
        }
    }
}
