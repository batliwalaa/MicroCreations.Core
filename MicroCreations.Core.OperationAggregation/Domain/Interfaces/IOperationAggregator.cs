namespace MicroCreations.Core.OperationAggregation.Domain.Interfaces
{
    public interface IOperationAggregator
    {
        BatchOperationResponse Execute(BatchOperationRequest request);
    }
}
