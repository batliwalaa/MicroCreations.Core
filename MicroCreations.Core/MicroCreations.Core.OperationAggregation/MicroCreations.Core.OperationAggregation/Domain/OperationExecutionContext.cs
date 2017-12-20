using System.Collections.Generic;
using System.Threading;

namespace MicroCreations.Core.OperationAggregation.Domain
{
    public class OperationExecutionContext
    {
        public IEnumerable<OperationArgument> Arguments { get; set; }

        public IEnumerable<OperationResult> Results { get; set; }

        public CancellationToken CancellationToken { get; set; }
    }
}
