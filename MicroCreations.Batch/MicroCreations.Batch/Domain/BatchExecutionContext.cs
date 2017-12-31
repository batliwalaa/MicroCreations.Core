using System.Collections.Generic;
using System.Threading;
using MicroCreations.Batch.Domain.Interfaces;

namespace MicroCreations.Batch.Domain
{
    public class BatchExecutionContext
    {
        internal BatchExecutionContext(
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

        // ReSharper disable once MemberCanBePrivate.Global
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public IEnumerable<OperationResult> Results { get; }

        // ReSharper disable once MemberCanBePrivate.Global
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public CancellationToken CancellationToken { get; }

        // ReSharper disable once MemberCanBePrivate.Global
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public IContext Context { get; }
    }
}
