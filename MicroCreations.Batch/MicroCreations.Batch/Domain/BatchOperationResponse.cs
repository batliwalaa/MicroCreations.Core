using System.Collections.Generic;

namespace MicroCreations.Batch.Domain
{
    public class BatchOperationResponse
    {
        public IEnumerable<OperationResult> Results { get; set; }
    }
}
