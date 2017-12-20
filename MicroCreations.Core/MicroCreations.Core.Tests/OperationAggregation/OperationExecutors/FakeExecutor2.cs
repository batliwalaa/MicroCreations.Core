using MicroCreations.Core.OperationAggregation.Domain;
using MicroCreations.Core.OperationAggregation.Domain.Interfaces;
using MicroCreations.Core.OperationAggregation.Extensions;

namespace MicroCreations.Core.Tests.OperationAggregation.OperationExecutors
{
    public class FakeExecutor2 : IOperationExecutor
    {
        public FakeExecutor2()
        {
            SupportedOperationName = "Operation 2";
        }
        public string SupportedOperationName { get; }

        public OperationResult Execute(OperationExecutionContext context)
        {
            return new OperationResult { Value = context.Arguments.Get("Operation Arg 2").Value, OperationName = SupportedOperationName };
        }
    }
}
