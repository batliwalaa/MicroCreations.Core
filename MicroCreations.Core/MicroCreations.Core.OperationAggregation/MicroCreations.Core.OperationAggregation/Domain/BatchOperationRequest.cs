using MicroCreations.Core.OperationAggregation.Enums;
using System.Collections.Generic;

namespace MicroCreations.Core.OperationAggregation.Domain
{
    public class BatchOperationRequest
    {
        public IEnumerable<OperationArgument> Arguments { get; set; }

        public IEnumerable<Operation> Operations { get; set; }

        public FaultCancellationOption FaultCancellationOption { get; set; }
    }
}
