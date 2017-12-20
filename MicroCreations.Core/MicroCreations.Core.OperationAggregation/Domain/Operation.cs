using MicroCreations.Core.OperationAggregation.Enums;

namespace MicroCreations.Core.OperationAggregation.Domain
{
    public class Operation
    {
        public string OperationName { get; set; }

        public ProcessingType ProcessingType { get; set; }
    }
}
