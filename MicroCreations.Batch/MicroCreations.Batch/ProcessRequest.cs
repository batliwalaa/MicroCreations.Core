using System.Collections.Generic;
using MicroCreations.Batch.Context;
using MicroCreations.Batch.Operations;

namespace MicroCreations.Batch
{
    public class ProcessRequest
    {
        public IEnumerable<IOperationExecutor> Executors { get; set; }

        public BatchOperationRequest Request { get; set; }

        public IContext ApplicationContext { get; set; }
    }
}
