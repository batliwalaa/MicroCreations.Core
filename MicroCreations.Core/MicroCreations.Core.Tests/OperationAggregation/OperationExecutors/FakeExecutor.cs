using System;
using System.Threading;
using MicroCreations.Core.OperationAggregation.Domain;
using MicroCreations.Core.OperationAggregation.Domain.Interfaces;
using MicroCreations.Core.OperationAggregation.Extensions;

namespace MicroCreations.Core.Tests.OperationAggregation.OperationExecutors
{
    public class FakeExecutor : IOperationExecutor
    {
        public FakeExecutor()
        {
            SupportedOperationName = "Operation 1";
        }

        public string SupportedOperationName { get; }

        public OperationResult Execute(OperationExecutionContext context)
        {
            Console.WriteLine("ThreadId: " + Thread.CurrentThread.ManagedThreadId);
            return new OperationResult { Value = context.Arguments.Get("Operation Arg 1").Value + " " + Thread.CurrentThread.ManagedThreadId, OperationName = SupportedOperationName };
        }
    }
}
