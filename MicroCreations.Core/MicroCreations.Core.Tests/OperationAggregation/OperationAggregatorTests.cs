using System.Collections.Generic;
using System.Linq;
using MicroCreations.Core.OperationAggregation;
using MicroCreations.Core.OperationAggregation.Domain;
using MicroCreations.Core.OperationAggregation.Domain.Interfaces;
using MicroCreations.Core.OperationAggregation.Enums;
using MicroCreations.Core.OperationAggregation.Extensions;
using MicroCreations.Core.Tests.OperationAggregation.OperationExecutors;
using Moq;
using NUnit.Framework;

namespace MicroCreations.Core.Tests.OperationAggregation
{
    [TestFixture]
    public class OperationAggregatorTests
    {
        private IOperationAggregator _operationAggregator;
        private Mock<IContextBuilder> _mockContextBuilder;

        [SetUp]
        public void Setup()
        {
            _mockContextBuilder = new Mock<IContextBuilder>();

            _operationAggregator = new OperationAggregator(new List<IOperationExecutor>
            {
                new FakeExecutor(),
                new FakeExecutor2(),
                new FakeExecutor3(),
                new FaultedExecutor()
            }, _mockContextBuilder.Object);

            _mockContextBuilder.Setup(x => x.GetContext()).Returns(default(IContext)).Verifiable();
        }
        
        [Test]
        public void OperationAggregator()
        {
            var expected = new BatchOperationResponse
            {
                Results = new List<OperationResult>
                {
                    new OperationResult
                    {
                        Value = "1"
                    }
                }
            };

            var request = new BatchOperationRequest
            {
                Operations = CreateOperations(ProcessingType.Serial),
                Arguments = new List<OperationArgument> { new OperationArgument { Name = "Operation Arg 1", Value = "1" }}
            };

            var result = _operationAggregator.Execute(request);
            
            Assert.AreEqual(expected.Results.First().Value, result.Results.First().Value);

            Verify();
        }
        
        [Test]
        public void OperationAggregatorUsingParallel()
        {
            var expected = new BatchOperationResponse
            {
                Results = new List<OperationResult> { new OperationResult { Value = "1", OperationName = "Operation 1"}, new OperationResult { Value = "2", OperationName = "Operation 2"} }
            };

            var request = new BatchOperationRequest
            {
                Operations = CreateOperations(ProcessingType.Parallel).Concat(new[] { new Operation { OperationName = "Operation 2", ProcessingType = ProcessingType.Parallel} }),
                Arguments = new List<OperationArgument>
                {
                    new OperationArgument { Name = "Operation Arg 1", Value = "1" },
                    new OperationArgument { Name = "Operation Arg 2", Value = "2" }
                }
            };

            var result = _operationAggregator.Execute(request);

            Assert.AreEqual(2, result.Results.Count());
            Assert.AreEqual(expected.Results.Get("Operation 1").Value, result.Results.Get("Operation 1").Value);
            Assert.AreEqual(expected.Results.Get("Operation 2").Value, result.Results.Get("Operation 2").Value);

            Verify();
        }

        [Test]
        public void OperationAggregatorUsingParallelWithOneFaulted()
        {
            var expected = new BatchOperationResponse
            {
                Results = new List<OperationResult>
                {
                    new OperationResult { Value = "1", OperationName = "Operation 1" },
                    new OperationResult { Value = "2", OperationName = "Operation 2" },
                    new OperationResult { Value = "2", OperationName = "FaultedExecutor", IsFaulted = true }
                }
            };

            var request = new BatchOperationRequest
            {
                Operations = CreateOperations(ProcessingType.Parallel).Concat(new[]
                {
                    new Operation { OperationName = "Operation 2", ProcessingType = ProcessingType.Parallel },
                    new Operation { OperationName = "FaultedExecutor", ProcessingType = ProcessingType.Parallel },
                }),
                Arguments = new List<OperationArgument>
                {
                    new OperationArgument { Name = "Operation Arg 1", Value = "1" },
                    new OperationArgument { Name = "Operation Arg 2", Value = "2" }
                }
            };

            var result = _operationAggregator.Execute(request);

            Assert.AreEqual(3, result.Results.Count());
            Assert.AreEqual(expected.Results.Get("Operation 1").Value, result.Results.Get("Operation 1").Value);
            Assert.AreEqual(expected.Results.Get("Operation 2").Value, result.Results.Get("Operation 2").Value);
            Assert.AreEqual(true, result.Results.Get("FaultedExecutor").IsFaulted);

            Verify();
        }

        private static IEnumerable<Operation> CreateOperations(ProcessingType processingType)
        {
            var operations = new List<Operation>();

            var op1 = new Operation
            {
                OperationName = "Operation 1",
                ProcessingType = processingType
            };
            
            operations.Add(op1);

            return operations;
        }

        private void Verify()
        {
            _mockContextBuilder.Verify(x => x.GetContext(), Times.Once);
        }
    }
}