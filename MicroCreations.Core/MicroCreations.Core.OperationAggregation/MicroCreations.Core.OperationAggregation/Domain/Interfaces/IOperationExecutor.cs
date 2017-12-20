namespace MicroCreations.Core.OperationAggregation.Domain.Interfaces
{
    public interface IOperationExecutor
    {
        string SupportedOperationName { get; set; }

        OperationResult Execute(OperationExecutionContext context);
    }
}
