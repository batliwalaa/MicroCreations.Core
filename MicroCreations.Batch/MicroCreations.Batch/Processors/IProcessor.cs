using System.Collections.Generic;
using System.Threading.Tasks;
using MicroCreations.Batch.Domain;
using MicroCreations.Batch.Enums;

namespace MicroCreations.Batch.Processors
{
    public interface IProcessor
    {
        Task<IEnumerable<OperationResult>> ProcessAsync(ProcessRequest processRequest);

        ProcessingType ProcessingType { get; }
    }
}
