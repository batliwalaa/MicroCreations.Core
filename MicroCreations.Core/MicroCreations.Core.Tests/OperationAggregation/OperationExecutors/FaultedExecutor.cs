using System;
using MicroCreations.Core.OperationAggregation.Domain;
using MicroCreations.Core.OperationAggregation.Domain.Interfaces;

namespace MicroCreations.Core.Tests.OperationAggregation.OperationExecutors
{
    public class FaultedExecutor : IOperationExecutor
    {
        public FaultedExecutor()
        {
            SupportedOperationName = "FaultedExecutor";
        }

        public string SupportedOperationName { get; }

        public OperationResult Execute(OperationExecutionContext context)
        {
            throw new NotImplementedException();
        }
    }
}
