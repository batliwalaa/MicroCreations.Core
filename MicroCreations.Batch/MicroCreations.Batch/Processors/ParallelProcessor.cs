using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using MicroCreations.Batch.Common;
using MicroCreations.Batch.Common.Operations;

namespace MicroCreations.Batch.Processors
{
    internal class ParallelProcessor : BaseProcessor
    {
        public ParallelProcessor(IEnumerable<IOperationExecutor> executors)
               : base(executors)
        {
        }
        public override ProcessorType ProcessorType => ProcessorType.Parallel;

        public override async Task<IEnumerable<OperationResult>> ProcessAsync(ProcessRequest processRequest)
        {
            var results = new List<OperationResult>(processRequest.Results);
            var executorTransformBlock = GetExecutorTransformBlock(results, processRequest);
            // ReSharper disable once ImplicitlyCapturedClosure
            // TODO: look into implicitly captured closure warning
            var resultActionBlock = new ActionBlock<OperationResult>(r => results.Add(r));

            executorTransformBlock.LinkTo(resultActionBlock, new DataflowLinkOptions
            {
                PropagateCompletion = true
            });

            var executors = GetExecutors(processRequest.Operations).ToList();
            var executorCount = executors.Count;

            for (var i = 0; i < executorCount; ++i)
            {
                executorTransformBlock.Post(executors[i]);
            }

            executorTransformBlock.Complete();

            await resultActionBlock.Completion;

            return results;
        }

        private static TransformBlock<IOperationExecutor, OperationResult> GetExecutorTransformBlock(ICollection<OperationResult> results, ProcessRequest processRequest)
        {
            var cancellationTokenSource = new CancellationTokenSource();

            return new TransformBlock<IOperationExecutor, OperationResult>(
                async operationExecutor =>
                {
                    var result = default(OperationResult);
                    try
                    {
                        if (!cancellationTokenSource.Token.IsCancellationRequested)
                        {
                            result = await operationExecutor.Execute(GetBatchExecutionContext(
                                processRequest.Arguments,
                                processRequest.ApplicationContext, results, cancellationTokenSource.Token));
                        }
                    }
                    catch (Exception ex)
                    {
                        if (processRequest.FaultCancellationOption == FaultCancellationOption.Cancel)
                        {
                            cancellationTokenSource.Cancel();
                        }
                        results.Add(GetFaultedOperationResult(operationExecutor.SupportedOperationName, ex));
                    }

                    return result;

                }, new ExecutionDataflowBlockOptions
                {
                    CancellationToken = cancellationTokenSource.Token,
                    MaxDegreeOfParallelism = MaxDegreeOfParallelism
                });
        }

        private static int MaxDegreeOfParallelism
        {
            get
            {
                var maxDegreeofParallelism =
                    ToNullableInt(ConfigurationManager.AppSettings["MicroCreations_Batch_MaxDegreeOfParallelism"]);

                if (!maxDegreeofParallelism.HasValue)
                {
                    maxDegreeofParallelism = DataflowBlockOptions.Unbounded;
                }

                return maxDegreeofParallelism.Value;
            }
        }
    }
}