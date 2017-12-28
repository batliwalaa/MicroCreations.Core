using System.Collections.Generic;
using System.Threading;
using MicroCreations.Core.OperationAggregation.Domain.Interfaces;

namespace MicroCreations.Core.OperationAggregation.Domain
{
    public class OperationExecutionContext
    {
        internal OperationExecutionContext(
            IEnumerable<OperationArgument> arguments,
            IEnumerable<OperationResult> results,
            CancellationToken cancellationToken,
            IContext context)
        {
            Arguments = arguments;
            Results = results;
            CancellationToken = cancellationToken;
            Context = context;
        }

        public IEnumerable<OperationArgument> Arguments { get; }

        public IEnumerable<OperationResult> Results { get; }

        public CancellationToken CancellationToken { get; }

        public IContext Context { get; }
    }
}
