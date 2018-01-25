using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MicroCreations.Batch.Common;
using MicroCreations.Batch.Common.Context;
using MicroCreations.Batch.Common.Operations;

namespace MicroCreations.Batch.Processors
{
    internal abstract class BaseProcessor : IProcessor
    {
        private readonly IEnumerable<IOperationExecutor> _executors;

        protected BaseProcessor(IEnumerable<IOperationExecutor> executors)
        {
            _executors = executors;
        }

        public abstract Task<IEnumerable<OperationResult>> ProcessAsync(ProcessRequest processRequest);

        public abstract ProcessorType ProcessorType { get; }

        protected static BatchExecutionContext GetBatchExecutionContext(
            IEnumerable<OperationArgument> arguments,
            IContext context,
            IEnumerable<OperationResult> results,
            CancellationToken cancellationToken)
        {
            return new BatchExecutionContext(arguments, results, cancellationToken, context);
        }

        [ExcludeFromCodeCoverage]
        protected static int? ToNullableInt(string value)
        {
            var result = default(int?);

            if (int.TryParse(value, out var intValue))
            {
                result = intValue;
            }

            return result;
        }

        protected static OperationResult GetFaultedOperationResult(string operationName, Exception exception)
        {
            return new OperationResult
            {
                Exception = new BatchException(exception.Message),
                IsFaulted = true,
                OperationName = operationName
            };
        }

        protected IEnumerable<IOperationExecutor> GetExecutors(IEnumerable<Operation> operations)
        {
            var operationNames = operations.Select(x => x.OperationName).ToList();

            return _executors.Where(x => operationNames.Contains(x.SupportedOperationName)).OrderBy(x => operationNames.IndexOf(x.SupportedOperationName));
        }
    }
}