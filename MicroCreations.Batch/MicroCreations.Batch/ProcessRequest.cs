using System.Collections.Generic;
using MicroCreations.Batch.Common;
using MicroCreations.Batch.Common.Context;
using MicroCreations.Batch.Common.Operations;

namespace MicroCreations.Batch
{
    public class ProcessRequest
    {
        public ProcessRequest() : this(null) { }

        public ProcessRequest(IEnumerable<OperationResult> results)
        {
            Results = results ?? new OperationResult[] { };
        }

        public IEnumerable<Operation> Operations { get; set; }

        public FaultCancellationOption FaultCancellationOption { get; set; }

        public IEnumerable<OperationArgument> Arguments { get; set; }

        public IContext ApplicationContext { get; set; }

        public IEnumerable<OperationResult> Results { get; }
    }
}
