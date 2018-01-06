using System.Collections.Generic;
using MicroCreations.Batch.Operations;

namespace MicroCreations.Batch.Builders
{
    public interface IRequestBuilder<out T>
    {
        T Supports { get; }

        BatchOperationRequest Build();

        BatchOperationRequest Build(IEnumerable<OperationArgument> operationArguments);
    }
}
