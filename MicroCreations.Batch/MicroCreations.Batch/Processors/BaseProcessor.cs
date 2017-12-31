using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MicroCreations.Batch.Domain;
using MicroCreations.Batch.Domain.Interfaces;
using MicroCreations.Batch.Enums;

namespace MicroCreations.Batch.Processors
{
    internal abstract class BaseProcessor : IProcessor
    {
        public abstract Task<IEnumerable<OperationResult>> ProcessAsync(ProcessRequest processRequest);

        public abstract ProcessingType ProcessingType { get; }

        protected static BatchExecutionContext GetBatchExecutionContext(
            IEnumerable<OperationArgument> arguments,
            IContext context,
            IEnumerable<OperationResult> results,
            CancellationToken cancellationToken)
        {
            return new BatchExecutionContext(arguments, results, cancellationToken, context);
        }

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
    }
}