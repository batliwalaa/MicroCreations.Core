using System.Collections.Generic;

namespace MicroCreations.Batch.Operations
{
    public class BatchOperationResponse
    {
        public IEnumerable<OperationResult> Results { get; set; }
    }
}
