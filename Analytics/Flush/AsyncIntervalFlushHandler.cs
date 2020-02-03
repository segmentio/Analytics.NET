#if !NET35
using Segment;
using Segment.Flush;
using Segment.Model;
using Segment.Request;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Segment.Flush
{

    internal class AsyncIntervalFlushHandler : IFlushHandler
    {

        private readonly ConcurrentQueue<BaseAction> _queue;
        private readonly int _maxBatchSize;
        private readonly IBatchFactory _batchFactory;
        private readonly IRequestHandler _requestHandler;
        private readonly int _maxQueueSize;
        private readonly CancellationTokenSource _continue;
        private readonly int _flushIntervalInMillis;
        private const int _workloads = 4;
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(_workloads);

        internal AsyncIntervalFlushHandler(IBatchFactory batchFactory,
            IRequestHandler requestHandler,
            int maxQueueSize,
            int maxBatchSize, 
            int flushIntervalInMillis)
        {
            _queue = new ConcurrentQueue<BaseAction>();
            _batchFactory = batchFactory;
            _requestHandler = requestHandler;
            _maxQueueSize = maxQueueSize;
            _maxBatchSize = maxBatchSize;
            _continue = new CancellationTokenSource();
            _flushIntervalInMillis = flushIntervalInMillis;

            _ = RunInterval();
        }

        private async Task RunInterval()
        {
            while (!_continue.Token.IsCancellationRequested)
            {
                _ = Task.Run(NewMethod);
                await Task.Delay(_flushIntervalInMillis);
            }
        }

        private async Task NewMethod()
        {
            if (_semaphore.CurrentCount <= 0) {
                Logger.Debug("Skipping flush. Workload limit has been reached");
                return;
            }

            try
            {
                await _semaphore.WaitAsync();
                await FlushImpl();
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public void Flush()
        {
            try
            {
                _semaphore.Wait();
                FlushImpl().GetAwaiter().GetResult();
            }
            finally
            {
                _semaphore.Release();
            }

            for (var i = 0; i < _workloads; i++) _semaphore.Wait();
            for (var i = 0; i < _workloads; i++) _semaphore.Release();

        }

        private async Task FlushImpl()
        {
            var current = new List<BaseAction>();
            while (!_queue.IsEmpty && !_continue.Token.IsCancellationRequested)
            {
                do
                {                 
                    if (!_queue.TryDequeue(out var action)) break;

                    Logger.Debug("Dequeued action in async loop.", new Dict{
                            { "message id", action.MessageId },
                            { "queue size", _queue.Count }
                         });

                    current.Add(action);
                } while (!_queue.IsEmpty && current.Count <= _maxBatchSize && !_continue.Token.IsCancellationRequested);

                if (current.Count > 0)
                {
                    // we have a batch that we're trying to send
                    Batch batch = _batchFactory.Create(current);

                    Logger.Debug("Created flush batch.", new Dict {
                        { "batch size", current.Count }
                    });

                    // make the request here
                    await _requestHandler.MakeRequest(batch);

                    // mark the current batch as null
                    current = new List<BaseAction>();
                }
            }
        }

        public Task Process(BaseAction action)
        {
            //todo: verify what to do when queue is full
            _queue.Enqueue(action);

            Logger.Debug("Enqueued action in async loop.", new Dict{
                            { "message id", action.MessageId },
                            { "queue size", _queue.Count }
                         });

            return Task.FromResult(true);
        }

        public void Dispose()
        {
            Logger.Debug("Disposing AsyncIntervalFlushHandler");
            _semaphore.Dispose();
            _continue.Cancel();
        }

    }
}
#endif