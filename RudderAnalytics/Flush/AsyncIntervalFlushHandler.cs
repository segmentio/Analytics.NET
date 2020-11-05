using RudderStack.Model;
using RudderStack.Request;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RudderStack.Flush
{

    internal class AsyncIntervalFlushHandler : IAsyncFlushHandler
    {
        /// <summary>
        /// Our servers only accept payloads smaller than 32KB
        /// </summary>
        private const int ActionMaxSize = 32 * 1024;

        /// <summary>
        /// Our servers only accept request smaller than 512KB we left 12kb as margin error
        /// </summary>
        private const int BatchMaxSize = 500 * 1024;

        private readonly ConcurrentQueue<BaseAction> _queue;
        private readonly int _maxBatchSize;
        private readonly IBatchFactory _batchFactory;
        private readonly IRequestHandler _requestHandler;
        private readonly int _maxQueueSize;
        private readonly CancellationTokenSource _continue;
        private readonly int _flushIntervalInMillis;
        private readonly int _threads;
        private readonly Semaphore _semaphore;
        private Timer _timer;

        internal AsyncIntervalFlushHandler(IBatchFactory batchFactory,
            IRequestHandler requestHandler,
            int maxQueueSize,
            int maxBatchSize,
            int flushIntervalInMillis,
            int threads)
        {
            _queue = new ConcurrentQueue<BaseAction>();
            _batchFactory = batchFactory;
            _requestHandler = requestHandler;
            _maxQueueSize = maxQueueSize;
            _maxBatchSize = maxBatchSize;
            _continue = new CancellationTokenSource();
            _flushIntervalInMillis = flushIntervalInMillis;
            _threads = threads;
            _semaphore = new Semaphore(_threads, _threads);

            RunInterval();
        }

        private void RunInterval()
        {
            var initialDelay = _queue.Count == 0 ? _flushIntervalInMillis : 0;
            _timer = new Timer(new TimerCallback(async (b) => await PerformFlush()), new { }, initialDelay, _flushIntervalInMillis);
        }


        private async Task PerformFlush()
        {
            if (!_semaphore.WaitOne(1))
            {
                Logger.Debug("Skipping flush. Workload limit has been reached");
                return;
            }

            try
            {
                await FlushImpl();
            }
            catch
            {
                Logger.Error("Flush couldn't be completed");
            }
            finally
            {
                _semaphore.Release();
            }
        }

        /// <summary>
        /// Blocks until all the messages are flushed
        /// </summary>
        public void Flush()
        {
            FlushAsync().GetAwaiter().GetResult();
        }

        public async Task FlushAsync()
        {
            await PerformFlush().ConfigureAwait(false);
            WaitWorkersToBeReleased();
        }

        private void WaitWorkersToBeReleased()
        {
            for (var i = 0; i < _threads; i++) _semaphore.WaitOne();
            _semaphore.Release(_threads);
        }

        private async Task FlushImpl()
        {
            var current = new List<BaseAction>();
            var currentSize = 0;
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
                    currentSize += action.Size;
                } while (!_queue.IsEmpty && current.Count < _maxBatchSize && !_continue.Token.IsCancellationRequested && currentSize < BatchMaxSize - ActionMaxSize);

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
                    currentSize = 0;
                }
            }
        }

        public async Task Process(BaseAction action)
        {
            action.Size = ActionSizeCalculator.Calculate(action);

            if (action.Size > ActionMaxSize)
            {
                Logger.Error($"Action was dropped cause is bigger than {ActionMaxSize} bytes");
                return;
            }

            _queue.Enqueue(action);

            Logger.Debug("Enqueued action in async loop.", new Dict{
                            { "message id", action.MessageId },
                            { "queue size", _queue.Count }
                         });

            if (_queue.Count >= _maxQueueSize)
            {
                Logger.Debug("Queue is full. Performing a flush");
                _ = PerformFlush();
            }

        }

        public void Dispose()
        {
            Logger.Debug("Disposing AsyncIntervalFlushHandler");
            _timer?.Dispose();
#if !NET35
            _semaphore?.Dispose();
#endif
            _continue?.Cancel();
        }

    }
}
