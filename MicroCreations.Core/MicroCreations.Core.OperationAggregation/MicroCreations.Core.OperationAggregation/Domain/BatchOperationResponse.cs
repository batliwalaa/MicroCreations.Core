using System.Collections.Generic;

namespace MicroCreations.Core.OperationAggregation.Domain
{
    public class BatchOperationResponse
    {
        public IEnumerable<OperationResult> Results { get; set; }
    }
}
