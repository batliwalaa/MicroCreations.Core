using System;

namespace MicroCreations.Core.OperationAggregation.Domain
{
    public class OperationResult
    {
        public string OperationName { get; set; }

        public object Value { get; set; }

        public bool IsFaulted { get; set; }

        public Exception Exception { get; set; }

        public T GetValue<T>()
    }
}
