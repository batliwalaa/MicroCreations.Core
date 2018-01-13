using System.Threading.Tasks;
using MicroCreations.Batch.Common.Operations;

namespace MicroCreations.Batch
{
    public interface IBatchAggregator
    {
        Task<BatchOperationResponse> Execute(BatchOperationRequest request);
    }
}
