using System.Threading.Tasks;
using MicroCreations.Batch.Operations;

namespace MicroCreations.Batch
{
    public interface IBatchAggregator
    {
        Task<BatchOperationResponse> Execute(BatchOperationRequest request);
    }
}
