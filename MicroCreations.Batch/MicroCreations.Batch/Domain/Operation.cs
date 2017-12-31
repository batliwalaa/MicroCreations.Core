using MicroCreations.Batch.Enums;

namespace MicroCreations.Batch.Domain
{
    public class Operation
    {
        public string OperationName { get; set; }

        public ProcessingType ProcessingType { get; set; }
    }
}
