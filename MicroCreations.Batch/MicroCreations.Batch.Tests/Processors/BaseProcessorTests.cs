using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using MicroCreations.Batch.Context;
using MicroCreations.Batch.Operations;
using MicroCreations.Batch.Processors;
using Moq;
using NUnit.Framework;

namespace MicroCreations.Batch.Tests.Processors
{
    [TestFixture]
    public abstract class BaseProcessorTests
    {
        private Mock<IOperationExecutor> _operationExecutorMock;
        protected IProcessor Processor { get; set; }

        [SetUp]
        public void SetUp()
        {
            Initialise();
        }

        protected void SetUpMocks(bool throwsException = false)
        {
            _operationExecutorMock = new Mock<IOperationExecutor>();
            var setup = _operationExecutorMock.Setup(x => x.Execute(It.IsAny<BatchExecutionContext>()));
            if (throwsException)
            {
                setup.Throws<Exception>().Verifiable();
            }
            else
            {
                setup.Returns(Task.FromResult(new OperationResult())).Verifiable();
            }
        }

        protected abstract void Initialise();

        protected ProcessRequest GetRequest()
        {
            return new ProcessRequest
            {
                Executors = new[]
                {
                    _operationExecutorMock.Object
                },
                Request = new BatchOperationRequest
                {
                    FaultCancellationOption = FaultCancellationOption.Cancel
                }
            };
        }

        protected void Verify()
        {
            _operationExecutorMock.Verify(x => x.Execute(It.IsAny<BatchExecutionContext>()), Times.Once);
        }

        protected async Task Test_With_One_Executor_Expect_Result_With_Count_One()
        {
            SetUpMocks();

            var results = (await Processor.ProcessAsync(GetRequest())).ToList();

            results.Should().BeOfType<List<OperationResult>>();
            results.First().ShouldBeEquivalentTo(new OperationResult());
            results.Count().ShouldBeEquivalentTo(1);

            Verify();
        }

        public async Task Test_With_One_Executor_Throws_Exceptions_Expect_Faulted_Result()
        {
            SetUpMocks(true);

            var results = (await Processor.ProcessAsync(GetRequest())).ToList();

            results.Should().BeOfType<List<OperationResult>>();
            results.First().IsFaulted.ShouldBeEquivalentTo(true);
            results.Count().ShouldBeEquivalentTo(1);

            Verify();
        }
    }
}
