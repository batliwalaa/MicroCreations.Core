using System.Linq;
using MicroCreations.Core.OperationAggregation.Domain;
using MicroCreations.Core.OperationAggregation.Domain.Interfaces;

namespace MicroCreations.Core.Tests.OperationAggregation.OperationExecutors
{
    public class FakeExecutor3 : IOperationExecutor
    {
        public FakeExecutor3()
        {
            SupportedOperationName = "Operation 3";
        }
        public string SupportedOperationName { get; }

        public OperationResult Execute(OperationExecutionContext context)
        {
            return new OperationResult { Value = context.Arguments.First().Value };
        }
    }
}
