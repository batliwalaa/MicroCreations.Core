using System.Collections.Generic;

namespace MicroCreations.Batch.Domain.Interfaces
{
    public interface IRequestBuilder<out T>
    {
        T Supports { get; }

        BatchOperationRequest Build();

        BatchOperationRequest Build(IEnumerable<OperationArgument> operationArguments);
    }
}
