using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using MicroCreations.Batch.Common.Operations;

namespace MicroCreations.Batch.Common.Context
{
    [ExcludeFromCodeCoverage]
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
        
        public IEnumerable<OperationResult> Results { get; }

        public CancellationToken CancellationToken { get; }

        public IContext Context { get; }
    }
}
