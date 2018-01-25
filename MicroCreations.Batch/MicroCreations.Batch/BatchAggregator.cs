using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MicroCreations.Batch.Common;
using MicroCreations.Batch.Common.Context;
using MicroCreations.Batch.Common.Operations;
using MicroCreations.Batch.Processors;

namespace MicroCreations.Batch
{
    public class BatchAggregator : IBatchAggregator
    {
        private readonly IContextBuilder _contextBuilder;
        private readonly IProcessor _serialProcessor;
        private readonly IProcessor _parallelProcessor;
        private readonly IProcessor _dependencyProcessor;        

        public BatchAggregator(
            IEnumerable<IProcessor> processors,
            IContextBuilder contextBuilder = null)
        {
            _contextBuilder = contextBuilder;

            _serialProcessor = processors.First(x => x.ProcessorType == ProcessorType.Serial);
            _parallelProcessor = processors.First(x => x.ProcessorType == ProcessorType.Parallel);
            _dependencyProcessor = processors.First(x => x.ProcessorType == ProcessorType.Dependency);
        }
        
        public async Task<BatchOperationResponse> Execute(BatchOperationRequest request)
        {
            var context = await _contextBuilder?.GetContext() ?? null;
            var results = await ExecuteBatch(request, context);

            var dependencyGraphResults = await _dependencyProcessor.ProcessAsync(new ProcessRequest(results)
            {
                ApplicationContext = context,
                Operations = request.Operations,
                Arguments = request.Arguments
            });

            results.AddRange(dependencyGraphResults);            

            return new BatchOperationResponse { Results = results };
        }
        
        private async Task<List<OperationResult>> ExecuteBatch(BatchOperationRequest request, IContext context)
        {
            var adjacentGroups = GetAdjacentGroups(request.Operations);
            var results = new List<OperationResult>();
            
            foreach (var kvp in adjacentGroups)
            {
                var result = await (kvp.Key == ProcessingType.Serial ? _serialProcessor : _parallelProcessor).ProcessAsync(new ProcessRequest(results)
                {
                    Arguments = request.Arguments,
                    FaultCancellationOption = request.FaultCancellationOption,
                    Operations = kvp.Value,
                    ApplicationContext = context
                });

                results.AddRange(result);

                if (request.FaultCancellationOption == FaultCancellationOption.Cancel && results.Any(x => x.IsFaulted))
                {
                    break;
                }
            }

            return results;
        }

        private static IDictionary<ProcessingType, IEnumerable<Operation>> GetAdjacentGroups(IEnumerable<Operation> operations)
        {
            var dictionary = new Dictionary<ProcessingType, IEnumerable<Operation>>();
            var currentKey = default(ProcessingType);
            var list = new List<Operation>();

            foreach(var operation in operations)
            {
                if(operation.ProcessingType != currentKey)
                {
                    if (list.Any())
                    {
                        dictionary.Add(currentKey, list);
                        list = new List<Operation>();
                    }

                    currentKey = operation.ProcessingType;
                }

                list.Add(operation);
            }

            if (list.Any())
            {
                dictionary.Add(currentKey, list);

            }

            return dictionary;
        }
    }
}