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
        private readonly List<Task> _tasks = new List<Task>();
        private readonly IBatchFactory _batchFactory;
        private readonly IRequestHandler _requestHandler;
        private readonly int _maxQueueSize;
        private readonly CancellationTokenSource _continue;

        internal AsyncIntervalFlushHandler(IBatchFactory batchFactory,
            IRequestHandler requestHandler,
                                 int maxQueueSize,
                                 int maxBatchSize)
        {
            _queue = new ConcurrentQueue<BaseAction>();
            _batchFactory = batchFactory;
            _requestHandler = requestHandler;
            _maxQueueSize = maxQueueSize;
            _maxBatchSize = maxBatchSize;
            _continue = new CancellationTokenSource();

            RunInterval();
        }

        private async Task RunInterval()
        {
            while (!_continue.Token.IsCancellationRequested)
            {
                Logger.Debug($"Flushing at {DateTime.Now}");
                _tasks.RemoveAll(t => t.IsCompleted);

                _tasks.Add(Task.Run(() => FlushImpl()));
                await Task.Delay(5000);
            }
        }

        public void Flush()
        {
            if (_tasks.Count > 0) Task.WaitAll(_tasks.ToArray());
            
            FlushImpl().GetAwaiter().GetResult();

        }

        public async Task FlushImpl()
        {
            var current = new List<BaseAction>();

            while (!_queue.IsEmpty && !_continue.Token.IsCancellationRequested)
            {
                do
                {
                    _queue.TryDequeue(out var action);
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
            _queue.Enqueue(action);

            Logger.Debug("Dequeued action in async loop.", new Dict{
                            { "message id", action.MessageId },
                            { "queue size", _queue.Count }
                         });


            return Task.FromResult(true);
        }

        public void Dispose()
        {
            _continue.Cancel();
        }

    }
}
#endif