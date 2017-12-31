using System;

namespace MicroCreations.Batch.Domain
{
    public class OperationResult
    {
        public string OperationName { get; set; }

        public object Value { get; set; }

        public bool IsFaulted { get; set; }

        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public BatchException Exception { get; set; }
        
    }
}
