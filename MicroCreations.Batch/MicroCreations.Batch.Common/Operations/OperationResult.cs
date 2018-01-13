namespace MicroCreations.Batch.Common.Operations
{
    public class OperationResult
    {
        public string OperationName { get; set; }

        public object Value { get; set; }

        public bool IsFaulted { get; set; }
        
        public BatchException Exception { get; set; }
        
    }
}
