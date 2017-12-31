using System.Collections.Generic;
using MicroCreations.Batch.Domain.Interfaces;

namespace MicroCreations.Batch.Domain
{
    public class ProcessRequest
    {
        public IEnumerable<IOperationExecutor> Executors { get; set; }

        public BatchOperationRequest Request { get; set; }

        public IContext ApplicationContext { get; set; }
    }
}
