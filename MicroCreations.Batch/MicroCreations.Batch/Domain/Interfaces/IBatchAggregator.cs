using System.Threading.Tasks;

namespace MicroCreations.Batch.Domain.Interfaces
{
    public interface IBatchAggregator
    {
        Task<BatchOperationResponse> Execute(BatchOperationRequest request);
    }
}
