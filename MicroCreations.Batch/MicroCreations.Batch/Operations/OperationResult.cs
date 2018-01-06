namespace MicroCreations.Batch.Operations
{
    public class OperationResult
    {
        public string OperationName { get; set; }

        public object Value { get; set; }

        public bool IsFaulted { get; set; }
        
        public BatchException Exception { get; set; }
        
    }
}
