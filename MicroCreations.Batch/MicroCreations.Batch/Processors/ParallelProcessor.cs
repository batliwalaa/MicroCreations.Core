using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using MicroCreations.Batch.Domain;
using MicroCreations.Batch.Domain.Interfaces;
using MicroCreations.Batch.Enums;

namespace MicroCreations.Batch.Processors
{
    internal class ParallelProcessor : BaseProcessor
    {
        public override ProcessingType ProcessingType => ProcessingType.Parallel;

        public override async Task<IEnumerable<OperationResult>> ProcessAsync(ProcessRequest processRequest)
        {
            var results = new List<OperationResult>();
            var executorTransformBlock = GetExecutorTransformBlock(results, processRequest);
            // ReSharper disable once ImplicitlyCapturedClosure
            // TODO: look into implicitly captured closure warning
            var resultActionBlock = new ActionBlock<OperationResult>(r => results.Add(r));

            executorTransformBlock.LinkTo(resultActionBlock, new DataflowLinkOptions
            {
                PropagateCompletion = true
            });

            executorTransformBlock.Complete();

            await resultActionBlock.Completion;

            return results;
        }

        private static TransformBlock<IOperationExecutor, OperationResult> GetExecutorTransformBlock(List<OperationResult> results, ProcessRequest processRequest)
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
                                processRequest.Request.Arguments,
                                processRequest.ApplicationContext, results, cancellationTokenSource.Token));
                        }
                    }
                    catch (Exception ex)
                    {
                        if (processRequest.Request.FaultCancellationOption == FaultCancellationOption.Cancel)
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