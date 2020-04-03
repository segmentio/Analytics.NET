using Moq;
using NUnit.Framework;
using Segment.Flush;
using Segment.Model;
using Segment.Request;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Segment.Test.Flush
{
    [TestFixture]
    public class AsyncIntervalFlushHandlerTests
    {
        private const int MaxBatchSize = 512 * 1024;

        AsyncIntervalFlushHandler _handler;
        Mock<IRequestHandler> _mockRequestHandler;
        Func<Task> _requestHandlerBehavior;

        [SetUp]
        public void Init()
        {
            _requestHandlerBehavior = SingleTaskResponseBehavior(Task.CompletedTask);
            _mockRequestHandler = new Mock<IRequestHandler>();

            _mockRequestHandler.Setup(r => r.MakeRequest(It.IsAny<Batch>()))
                .Returns(() => _requestHandlerBehavior())
                .Verifiable();

            _handler = GetFlushHandler(100, 20, 2000);
            Logger.Handlers += LoggingHandler;

        }

        [TearDown]
        public void CleanUp()
        {
            _handler.Dispose();
            Logger.Handlers -= LoggingHandler;
        }

        [Test()]
        public void FlushDoesNotMakeARequestWhenThereAreNotEvents()
        {
            _handler.Flush();

            _mockRequestHandler.Verify(r => r.MakeRequest(It.IsAny<Batch>()), times: Times.Exactly(0));
        }

        [Test()]
        public void FlushMakesARequestWhenThereAreEvents()
        {
            _handler.Process(new Track(null, null, null, null));
            _handler.Flush();

            _mockRequestHandler.Verify(r => r.MakeRequest(It.IsAny<Batch>()), times: Times.Exactly(1));
        }


        [Test]
        public async Task IntervalFlushIsTriggeredPeriodically()
        {
            var interval = 600;
            _handler = GetFlushHandler(100, 20, interval);
            await Task.Delay(100);
            int trials = 5;

            for (int i = 0; i < trials; i++)
            {
                await _handler.Process(new Track(null, null, null, null));
                _mockRequestHandler.Verify(r => r.MakeRequest(It.IsAny<Batch>()), times: Times.Exactly(i));
                await Task.Delay(interval);
            }

            _mockRequestHandler.Verify(r => r.MakeRequest(It.IsAny<Batch>()), times: Times.Exactly(trials));
        }

        [Test]
        public async Task FlushSplitEventsInBatches()
        {
            var queueSize = 100;
            _handler = GetFlushHandler(queueSize, 20, 20000);
            await Task.Delay(100);

            for (int i = 0; i < queueSize; i++)
            {
                _ = _handler.Process(new Track(null, null, null, null));
            }

            _handler.Flush();

            _mockRequestHandler.Verify(r => r.MakeRequest(It.IsAny<Batch>()), times: Times.Exactly(5));
        }

        [Test]
        public async Task ProcessActionFlushWhenQueueIsFull()
        {
            var queueSize = 10;
            _handler = GetFlushHandler(queueSize, 20, 20000);
            await Task.Delay(50);

            for (int i = 0; i < queueSize + 1; i++)
            {
                _ = _handler.Process(new Track(null, null, null, null));
            }

            _mockRequestHandler.Verify(r => r.MakeRequest(It.IsAny<Batch>()), times: Times.Exactly(1));
        }

        [Test]
        public async Task FlushWaitsForPreviousFlushesTriggeredByInterval()
        {
            var time = 1500;
            _handler = GetFlushHandler(100, 20, 500);
            _requestHandlerBehavior = MultipleTaskResponseBehavior(Task.Delay(time));
            
            DateTime start = DateTime.Now;
            _ = _handler.Process(new Track(null, null, null, null));

            await Task.Delay(500);

            _handler.Flush();

            TimeSpan duration = DateTime.Now.Subtract(start);

            _mockRequestHandler.Verify(r => r.MakeRequest(It.IsAny<Batch>()), times: Times.Exactly(1));
            //50 millisecons as error margin
            Assert.IsTrue(duration.CompareTo(TimeSpan.FromMilliseconds(time - 50)) >= 0);

        }

        [Test]
        public async Task IntervalFlushLimitConcurrentProcesses ()
        {
            var time = 2000;
            _handler = GetFlushHandler(100, 20, 300);
            _requestHandlerBehavior = MultipleTaskResponseBehavior(Task.Delay(time), Task.CompletedTask, Task.Delay(time));

            _ = _handler.Process(new Track(null, null, null, null));
            await Task.Delay(400);

            for (int i = 0; i < 3; i++)
            {
                await _handler.Process(new Track(null, null, null, null));
                _mockRequestHandler.Verify(r => r.MakeRequest(It.IsAny<Batch>()), times: Times.Exactly(1));

                await Task.Delay(300);
            }

            _handler.Flush();

            _mockRequestHandler.Verify(r => r.MakeRequest(It.IsAny<Batch>()), times: Times.Exactly(2));

        }


        [Test]
        public void IntervalFlushSplitsBatchesThatAreBiggerThan512Kb()
        {
            _handler = GetFlushHandler(100, 100, 10000);

            var actions = GetActions(20, GetEventName(30 * 1024));

            foreach (var action in actions)
            {
                _ = _handler.Process(action);
            }

            _handler.Flush();

            _mockRequestHandler.Verify(r => r.MakeRequest(It.Is<Batch>(b => b.batch.Sum(a => a.Size) < MaxBatchSize)), times: Times.Exactly(2));
        }

        [Test]
        public void IntervalFlushSendsBatchesThatAreSmallerThan512Kb()
        {
            _handler = GetFlushHandler(1000, 1000, 10000);

            var actions = GetActions(999, GetEventName(30));

            foreach (var action in actions)
            {
                _ = _handler.Process(action);
            }

            _handler.Flush();

            _mockRequestHandler.Verify(r => r.MakeRequest(It.Is<Batch>(b => b.batch.Sum(a => a.Size) < MaxBatchSize)), times: Times.Exactly(1));


        }

        private string GetEventName(int size)
        {
            return string.Join("", Enumerable.Range(0, size).Select(_ => "a"));
        }

        private List<BaseAction> GetActions(int actionsNumber, string eventName)
        {
            return new List<BaseAction>(Enumerable.Range(0, actionsNumber).Select(n => new Track("user", eventName, null, null)));
        }

        private AsyncIntervalFlushHandler GetFlushHandler(int maxQueueSize, int maxBatchSize, int flushIntervalInMillis)
        {
            return new AsyncIntervalFlushHandler(new SimpleBatchFactory("TestKey"), _mockRequestHandler.Object, maxQueueSize, maxBatchSize, flushIntervalInMillis);
        }

        private Func<Task> SingleTaskResponseBehavior(Task task)
        {
            return () => task;
        }

        private Func<Task> MultipleTaskResponseBehavior(params Task[] tasks)
        {
            var response = new Queue<Task>(tasks);
            return () => response.Count > 0 ? response.Dequeue() : null;
        }
        static void LoggingHandler(Logger.Level level, string message, IDictionary<string, object> args)
        {
            if (args != null)
            {
                foreach (string key in args.Keys)
                {
                    message += String.Format(" {0}: {1},", "" + key, "" + args[key]);
                }
            }
            Console.WriteLine(String.Format("[FlushTests] [{0}] {1}", level, message));
        }
    }
}
