using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MicroCreations.Batch.Common;
using MicroCreations.Batch.Common.Context;
using MicroCreations.Batch.Common.Operations;
using MicroCreations.Batch.Processors;
using Moq;
using NUnit.Framework;
using FluentAssertions;

namespace MicroCreations.Batch.Tests
{
    [TestFixture]
    public class BatchAggregatorTests
    {
        private IBatchAggregator _operationAggregator;
        private Mock<IContextBuilder> _contextBuilderMock;
        private Mock<IProcessor> _serialProcessorMock;
        private Mock<IProcessor> _parallelProcessorMock;
        private Mock<IProcessor> _dependencyProcessorMock;
        private Mock<IOperationExecutor> _operationExecutorMock;

        [SetUp]
        public void Setup()
        {
            _contextBuilderMock = new Mock<IContextBuilder>();
            _operationExecutorMock = new Mock<IOperationExecutor>();
            _serialProcessorMock = new Mock<IProcessor>();
            _parallelProcessorMock = new Mock<IProcessor>();
            _dependencyProcessorMock = new Mock<IProcessor>();

            _contextBuilderMock.Setup(x => x.GetContext()).Returns(Task.FromResult(default(IContext))).Verifiable();
        }

        [Test]
        public async Task When_Execute_Invoked_With_1_Serial_Operation_Request_Expect_BatchOperationResponse_With_1_Result()
        {
            var operations = CreateOperations(new[] { ProcessingType.Serial });
            var expectedResult = new OperationResult { OperationName = "Operation 1", Value = 1 };

            _parallelProcessorMock.SetupGet(x => x.ProcessorType).Returns(ProcessorType.Parallel).Verifiable();
            _serialProcessorMock.SetupGet(x => x.ProcessorType).Returns(ProcessorType.Serial).Verifiable();
            _serialProcessorMock.Setup(x => x.ProcessAsync(It.IsAny<ProcessRequest>())).Returns(Task.FromResult(new[] { expectedResult }.AsEnumerable())).Verifiable();

            _operationAggregator = new BatchAggregator(new[] { _serialProcessorMock.Object, _parallelProcessorMock.Object, _dependencyProcessorMock.Object }, _contextBuilderMock.Object);

            var results = await _operationAggregator.Execute(new BatchOperationRequest { Operations = operations, FaultCancellationOption = FaultCancellationOption.None });

            results.Should().BeOfType<BatchOperationResponse>();
            results.Results.Count().ShouldBeEquivalentTo(1);
            results.Results.First().ShouldBeEquivalentTo(expectedResult);

            _serialProcessorMock.VerifyGet(x => x.ProcessorType, Times.Exactly(2));
            _serialProcessorMock.Verify(x => x.ProcessAsync(It.IsAny<ProcessRequest>()), Times.Once);

            Verify();
        }

        [Test]
        public async Task When_Execute_Invoked_With_1_Parallel_Operation_Request_Expect_BatchOperationResponse_With_1_Result()
        {
            var operations = CreateOperations(new[] { ProcessingType.Parallel });
            var expectedResult = new OperationResult { OperationName = "Operation 1", Value = 1 };

            _serialProcessorMock.SetupGet(x => x.ProcessorType).Returns(ProcessorType.Serial).Verifiable();
            _parallelProcessorMock.SetupGet(x => x.ProcessorType).Returns(ProcessorType.Parallel).Verifiable();
            _parallelProcessorMock.Setup(x => x.ProcessAsync(It.IsAny<ProcessRequest>())).Returns(Task.FromResult(new[] { expectedResult }.AsEnumerable())).Verifiable();

            _operationAggregator = new BatchAggregator(new[] { _parallelProcessorMock.Object, _serialProcessorMock.Object, _dependencyProcessorMock.Object }, _contextBuilderMock.Object);

            var results = await _operationAggregator.Execute(new BatchOperationRequest { Operations = operations, FaultCancellationOption = FaultCancellationOption.None });

            results.Should().BeOfType<BatchOperationResponse>();
            results.Results.Count().ShouldBeEquivalentTo(1);
            results.Results.First().ShouldBeEquivalentTo(expectedResult);

            _parallelProcessorMock.VerifyGet(x => x.ProcessorType, Times.Exactly(2));
            _parallelProcessorMock.Verify(x => x.ProcessAsync(It.IsAny<ProcessRequest>()), Times.Once);

            Verify();
        }
        
        [Test]
        public async Task When_Execute_Invoked_With_1_Serial_And_1_Parallel_Expect_BatchOperationResponse_With_2_Result()
        {
            var operations = CreateOperations(new[] { ProcessingType.Parallel, ProcessingType.Serial });
            var expectedResults = new[]
            {
                new OperationResult { OperationName = "Operation 1", Value = 1 },
                new OperationResult { OperationName = "Operation 2", Value = 2 },
            }.AsEnumerable();

            _serialProcessorMock.SetupGet(x => x.ProcessorType).Returns(ProcessorType.Serial).Verifiable();
            _serialProcessorMock.Setup(x => x.ProcessAsync(It.IsAny<ProcessRequest>())).Returns(Task.FromResult(new[] { new OperationResult { OperationName = "Operation 1", Value = 1 } }.AsEnumerable())).Verifiable();
            _parallelProcessorMock.SetupGet(x => x.ProcessorType).Returns(ProcessorType.Parallel).Verifiable();
            _parallelProcessorMock.Setup(x => x.ProcessAsync(It.IsAny<ProcessRequest>())).Returns(Task.FromResult(new[] { new OperationResult { OperationName = "Operation 2", Value = 2 } }.AsEnumerable())).Verifiable();

            _operationAggregator = new BatchAggregator(new[] { _serialProcessorMock.Object, _parallelProcessorMock.Object, _dependencyProcessorMock.Object }, _contextBuilderMock.Object);

            var results = await _operationAggregator.Execute(new BatchOperationRequest { Operations = operations, FaultCancellationOption = FaultCancellationOption.None });

            results.Should().BeOfType<BatchOperationResponse>();
            results.Results.Count().ShouldBeEquivalentTo(2);
            results.Results.ShouldBeEquivalentTo(expectedResults);

            _serialProcessorMock.VerifyGet(x => x.ProcessorType, Times.AtLeastOnce);
            _serialProcessorMock.Verify(x => x.ProcessAsync(It.IsAny<ProcessRequest>()), Times.Once);
            _parallelProcessorMock.VerifyGet(x => x.ProcessorType, Times.AtLeastOnce);
            _parallelProcessorMock.Verify(x => x.ProcessAsync(It.IsAny<ProcessRequest>()), Times.Once);

            Verify();
        }

        [Test]
        public async Task Wen_Execute_Invoked_With_1_Operation_Throwing_Exception_Expect_Operation_IsFaulted()
        {
            var operations = CreateOperations(new[] { ProcessingType.Parallel });
            var expectedResult = new OperationResult { OperationName = "Operation 1", IsFaulted = true };

            _serialProcessorMock.SetupGet(x => x.ProcessorType).Returns(ProcessorType.Serial).Verifiable();
            _parallelProcessorMock.SetupGet(x => x.ProcessorType).Returns(ProcessorType.Parallel).Verifiable();
            _parallelProcessorMock.Setup(x => x.ProcessAsync(It.IsAny<ProcessRequest>())).Returns(Task.FromResult(new[] { expectedResult }.AsEnumerable())).Verifiable();

            _operationAggregator = new BatchAggregator(new[] { _parallelProcessorMock.Object, _serialProcessorMock.Object, _dependencyProcessorMock.Object }, _contextBuilderMock.Object);

            var results = await _operationAggregator.Execute(new BatchOperationRequest { Operations = operations, FaultCancellationOption = FaultCancellationOption.Cancel });

            results.Should().BeOfType<BatchOperationResponse>();
            results.Results.Count().ShouldBeEquivalentTo(1);
            results.Results.First().ShouldBeEquivalentTo(expectedResult);

            _parallelProcessorMock.VerifyGet(x => x.ProcessorType, Times.Exactly(2));
            _parallelProcessorMock.Verify(x => x.ProcessAsync(It.IsAny<ProcessRequest>()), Times.Once);

            Verify();
        }

        private static IEnumerable<Operation> CreateOperations(IEnumerable<ProcessingType> processingType)
        {
            var i = 0;

            return processingType.Select(x => new Operation { OperationName = $"Operation {++i}", ProcessingType = x });
        }

        private void Verify()
        {
            _contextBuilderMock.Verify(x => x.GetContext(), Times.Once);
        }
    }
}