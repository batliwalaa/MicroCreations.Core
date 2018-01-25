using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using MicroCreations.Batch.Common;
using MicroCreations.Batch.Common.Operations;
using MicroCreations.Batch.Processors;
using NUnit.Framework;

namespace MicroCreations.Batch.Tests.Processors
{
    public class SerialProcessorTests : BaseProcessorTests
    {
        [Test]
        public void When_Get_Invoked_On_Property_ProcessingType_Expect_To_Be_Serial()
        {
            Processor.ProcessorType.ShouldBeEquivalentTo(ProcessorType.Serial);
        }

        [Test]
        public async Task When_ProcessAsync_Invoked_With_One_Executor_Expect_Result_With_Count_One()
        {
            await Test_With_One_Executor_Expect_Result_With_Count_One();
        }

        [Test]
        public async Task When_ProcessAsync_Invoked_With_One_Executor_Throws_Exceptions_Expect_Faulted_Result()
        {
            await Test_With_One_Executor_Throws_Exceptions_Expect_Faulted_Result();
        }

        protected override void Initialise(IEnumerable<IOperationExecutor> executors)
        {
            Processor = new SerialProcessor(executors);
            ProcessingType = ProcessingType.Serial;
        }
        
    }
}
