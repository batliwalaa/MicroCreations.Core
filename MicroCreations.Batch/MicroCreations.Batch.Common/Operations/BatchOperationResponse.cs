using System.Collections.Generic;

namespace MicroCreations.Batch.Common.Operations
{
    public class BatchOperationResponse
    {
        public IEnumerable<OperationResult> Results { get; set; }
    }
}
