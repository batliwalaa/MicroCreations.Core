using System;
using System.Threading;
using System.Threading.Tasks;
using MicroCreations.Batch.Domain;
using MicroCreations.Batch.Domain.Interfaces;
using MicroCreations.Batch.Extensions;

namespace MicroCreations.Batch.Tests.OperationExecutors
{
    public class FakeExecutor : IOperationExecutor
    {
        public FakeExecutor()
        {
            SupportedOperationName = "Operation 1";
        }

        public string SupportedOperationName { get; }

        public async Task<OperationResult> Execute(BatchExecutionContext context)
        {
            Console.WriteLine("ThreadId: " + Thread.CurrentThread.ManagedThreadId);
            return await Task.Factory.StartNew(() => new OperationResult { Value = context.Arguments.Get("Operation Arg 1").Value + " " + Thread.CurrentThread.ManagedThreadId, OperationName = SupportedOperationName });
        }
    }
}
