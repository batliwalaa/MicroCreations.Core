using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MicroCreations.Batch.Common.DependencyGraph;
using MicroCreations.Batch.Common.Operations;
using MicroCreations.Batch.Processors;
using Moq;
using NUnit.Framework;
using FluentAssertions;
using MicroCreations.Batch.Common;

namespace MicroCreations.Batch.Tests.Processors
{
    [TestFixture]
    public class DependencyProcessorTests
    {
        private Mock<IOperationExecutor> _operationExecutorMock;
        private Mock<IProcessor> _processorMock;
        private Mock<IDependencyGraph> _dependencyGraphMock;
        private IProcessor _processor;

        [SetUp]
        public void Setup()
        {
            _operationExecutorMock = new Mock<IOperationExecutor>();
            _processorMock = new Mock<IProcessor>();
            _dependencyGraphMock = new Mock<IDependencyGraph>();
        }

        [Test]
        public async Task When_ProcessAsync_Invoked_When_DependencyGraph_IsNull_Expect_Correct_Behaviour()
        {
            _processorMock.SetupGet(x => x.ProcessorType).Returns(ProcessorType.Parallel);
            _processor = new DependencyProcessor(new[] { _operationExecutorMock.Object }, new[] { _processorMock.Object });

            var result = await _processor.ProcessAsync(new ProcessRequest());

            result.Should().BeOfType<List<OperationResult>>();
            result.Count().ShouldBeEquivalentTo(0);

            _processorMock.VerifyGet(x => x.ProcessorType, Times.Once);
        }

        [Test]
        public async Task When_ProcessAsync_Invoked_Expect_Correct_Behaviour()
        {
            var expectedResult = new OperationResult { OperationName = "Operation 1", Value = 1 };
            _processorMock.SetupGet(x => x.ProcessorType).Returns(ProcessorType.Parallel);
            _processorMock.Setup(x => x.ProcessAsync(It.IsAny<ProcessRequest>())).Returns(Task.FromResult(new[] { expectedResult }.AsEnumerable())).Verifiable();
            _processor = new DependencyProcessor(new[] { _operationExecutorMock.Object }, new[] { _processorMock.Object }, _dependencyGraphMock.Object);

            var result = await _processor.ProcessAsync(GetRequest());

            result.Should().BeOfType<List<OperationResult>>();
            result.Count().ShouldBeEquivalentTo(1);

            _processorMock.VerifyGet(x => x.ProcessorType, Times.Once);
            _processorMock.Verify(x => x.ProcessAsync(It.IsAny<ProcessRequest>()), Times.Exactly(1));
        }

        private static ProcessRequest GetRequest()
        {
            return new ProcessRequest(default(IEnumerable<OperationResult>))
            {
                Operations = new[]
                {
                    new Operation { OperationName = "Operation 1", ProcessingType = ProcessingType.Parallel }
                },

                FaultCancellationOption = FaultCancellationOption.Cancel
            };
        }
    }
}
