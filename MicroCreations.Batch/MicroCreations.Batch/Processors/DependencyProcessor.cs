using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MicroCreations.Batch.Common;
using MicroCreations.Batch.Common.Context;
using MicroCreations.Batch.Common.DependencyGraph;
using MicroCreations.Batch.Common.Operations;

namespace MicroCreations.Batch.Processors
{
    internal class DependencyProcessor : BaseProcessor
    {
        private readonly IDependencyGraph _dependencyGraph;
        private readonly IProcessor _parallelProcessor;

        public DependencyProcessor(
            IEnumerable<IOperationExecutor> executors,
            IEnumerable<IProcessor> processors,
            IDependencyGraph dependencyGraph = null)
            : base(executors)
        {
            _dependencyGraph = dependencyGraph;
            _parallelProcessor = processors.First(x => x.ProcessorType == ProcessorType.Parallel);
        }

        public override ProcessorType ProcessorType => ProcessorType.Dependency;

        public override async Task<IEnumerable<OperationResult>> ProcessAsync(ProcessRequest processRequest)
        {
            var results = new List<OperationResult>(processRequest.Results);
            var graphDepth = MaxDependencyGraphDepth;

            if(_dependencyGraph != null && graphDepth > 0)
            {
                var dependencyNodes = default(IEnumerable<DependencyNode>);

                for(var i = 0; i < graphDepth; ++i)
                {
                    var batchExecutionContext = new BatchExecutionContext(processRequest.Arguments, results, CancellationToken.None, processRequest.ApplicationContext);
                    var operationNames = i == 0 ? processRequest.Operations.Select(x => x.OperationName) : dependencyNodes.Select(x => x.Key);

                    dependencyNodes = default(IEnumerable<DependencyNode>);
                    dependencyNodes = GetDependencyNodes(operationNames, batchExecutionContext);

                    var dependencyResults = await _parallelProcessor.ProcessAsync(new ProcessRequest(results)
                    {
                        ApplicationContext = processRequest.ApplicationContext,
                        Arguments = processRequest.Arguments,
                        FaultCancellationOption = processRequest.FaultCancellationOption,
                        Operations = dependencyNodes.Select(x => new Operation { OperationName = x.Key, ProcessingType = ProcessingType.Parallel })
                    });

                    results.AddRange(dependencyResults);
                }
            }

            return results;
        }

        private IEnumerable<DependencyNode> GetDependencyNodes(IEnumerable<string> operationNames, BatchExecutionContext batchExecutionContext)
        {
            return from operationName in operationNames
                   let dependencies = _dependencyGraph.ResolveChildren(operationName)
                   from dependent in dependencies
                   where dependent.DependencyOperationPredicate.SupportsOperation(dependent.Key) &&
                         dependent.DependencyOperationPredicate.AllowExecute(batchExecutionContext)
                   select dependent;
        }
        
        private static int MaxDependencyGraphDepth
        {
            get
            {
                var maxDependencyGraphDepth =
                    ToNullableInt(ConfigurationManager.AppSettings["MicroCreations_Batch_MaxDependencyGraphDepth"]);

                if (!maxDependencyGraphDepth.HasValue)
                {
                    maxDependencyGraphDepth = 1;
                }
                else if(maxDependencyGraphDepth.Value > 5)
                {
                    maxDependencyGraphDepth = 5;
                }

                return maxDependencyGraphDepth.Value;
            }
        }
    }
}
