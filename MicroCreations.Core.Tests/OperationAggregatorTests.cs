using System.Collections.Generic;
using System.Linq;
using MicroCreations.Core.OperationAggregation;
using MicroCreations.Core.OperationAggregation.Domain;
using MicroCreations.Core.OperationAggregation.Domain.Interfaces;
using MicroCreations.Core.OperationAggregation.Enums;
using Moq;
using NUnit.Framework;

namespace MicroCreations.Core.Tests
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
            _operationAggregator = new OperationAggregator(new List<IOperationExecutor>{ new FakeExecutor() }, _mockContextBuilder.Object);
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
                Arguments = new List<OperationArgument> { new OperationArgument { Name = "Operation Args", Value = "1" }}
            };

            var result = _operationAggregator.Execute(request);
            
            Assert.AreEqual(expected.Results.First().Value, result.Results.First().Value);
        }
        
        [Test]
        public void OperationAggregatorUsingParallel()
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
                Operations = CreateOperations(ProcessingType.Parallel),
                Arguments = new List<OperationArgument> { new OperationArgument { Name = "Operation Args", Value = "1" }}
            };

            var result = _operationAggregator.Execute(request);
            
            Assert.AreEqual(expected.Results.First().Value, result.Results.First().Value);
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
        
        private class FakeExecutor : IOperationExecutor
        {
            public FakeExecutor()
            {
                SupportedOperationName = "Operation 1";
            }
            
            public string SupportedOperationName { get; set; }
            
            public OperationResult Execute(OperationExecutionContext context)
            {
                return new OperationResult {Value = context.Arguments.First().Value};
            }
        }
    }
}