using MicroCreations.Batch.Enums;
using System.Collections.Generic;

namespace MicroCreations.Batch.Domain
{
    public class BatchOperationRequest
    {
        public IEnumerable<OperationArgument> Arguments { get; set; }

        public IEnumerable<Operation> Operations { get; set; }

        public FaultCancellationOption FaultCancellationOption { get; set; }
    }
}
