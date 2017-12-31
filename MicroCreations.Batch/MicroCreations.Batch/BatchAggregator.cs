using MicroCreations.Batch.Domain.Interfaces;
using System.Collections.Generic;
using MicroCreations.Batch.Domain;
using MicroCreations.Batch.Enums;
using System.Linq;
using System.Threading.Tasks;
using MicroCreations.Batch.Processors;

namespace MicroCreations.Batch
{
    public class BatchAggregator : IBatchAggregator
    {
        private readonly IEnumerable<IOperationExecutor> _executors;
        private readonly IContextBuilder _contextBuilder;
        private readonly IEnumerable<IProcessor> _processors;

        public BatchAggregator(
            IEnumerable<IOperationExecutor> executors,
            IContextBuilder contextBuilder,
            IEnumerable<IProcessor> processors)
        {
            _executors = executors;
            _contextBuilder = contextBuilder;
            _processors = processors;
        }
        
        public async Task<BatchOperationResponse> Execute(BatchOperationRequest request)
        {
            return await ExecuteBatch(request);
        }

        private async Task<BatchOperationResponse> ExecuteBatch(BatchOperationRequest request)
        {
            var adjacentGroups = GetAdjacentGroups(request.Operations);
            var results = new List<OperationResult>();
            var context = await _contextBuilder.GetContext();
            
            foreach (var kvp in adjacentGroups)
            {
                var result = await _processors.First(x => x.ProcessingType == kvp.Key).ProcessAsync(new ProcessRequest
                {
                    Request = request,
                    Executors = GetExecutors(kvp.Value),
                    ApplicationContext = context
                });

                results.AddRange(result);

                if (request.FaultCancellationOption == FaultCancellationOption.Cancel && results.Any(x => x.IsFaulted))
                {
                    break;
                }
            }

            return new BatchOperationResponse { Results = results };
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
        
        private IEnumerable<IOperationExecutor> GetExecutors(IEnumerable<Operation> operations)
        {
            var operationNames = operations.Select(x => x.OperationName);

            return _executors.Where(x => operationNames.Contains(x.SupportedOperationName));
        }
    }
}