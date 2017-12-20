namespace MicroCreations.Core.OperationAggregation.Domain.Interfaces
{
    public interface IOperationExecutor
    {
        string SupportedOperationName { get; }

        OperationResult Execute(OperationExecutionContext context);
    }
}
