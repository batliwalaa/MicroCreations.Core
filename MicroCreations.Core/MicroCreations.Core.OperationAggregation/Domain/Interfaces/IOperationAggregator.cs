using System.Threading.Tasks;

namespace MicroCreations.Core.OperationAggregation.Domain.Interfaces
{
    public interface IOperationAggregator
    {
        BatchOperationResponse Execute(BatchOperationRequest request);

        Task<BatchOperationResponse> ExecuteAsync(BatchOperationRequest request);
    }
}
