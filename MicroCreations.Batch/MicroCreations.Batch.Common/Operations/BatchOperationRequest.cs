using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace MicroCreations.Batch.Common.Operations
{
    [ExcludeFromCodeCoverage]
    public class BatchOperationRequest
    {
        public IEnumerable<OperationArgument> Arguments { get; set; }

        public IEnumerable<Operation> Operations { get; set; }

        public FaultCancellationOption FaultCancellationOption { get; set; }
    }
}
