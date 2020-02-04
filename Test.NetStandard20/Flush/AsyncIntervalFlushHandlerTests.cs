using Moq;
using NUnit.Framework;
using Segment.Flush;
using Segment.Model;
using Segment.Request;
using System.Threading.Tasks;

namespace Segment.Test.Flush
{
    [TestFixture]
    public class AsyncIntervalFlushHandlerTests
    {
        AsyncIntervalFlushHandler _handler;
        Mock<IRequestHandler> _mockRequestHandler;

        [SetUp]
        public void Init()
        {
            _mockRequestHandler = new Mock<IRequestHandler>(MockBehavior.Loose);

            _mockRequestHandler.Setup(r => r.MakeRequest(It.IsAny<Batch>()))
                .Returns(Task.CompletedTask)
                .Verifiable();
            _handler = new AsyncIntervalFlushHandler(new SimpleBatchFactory(""), _mockRequestHandler.Object, 100, 20, 2000);

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
            _handler = new AsyncIntervalFlushHandler(new SimpleBatchFactory(""), _mockRequestHandler.Object, 100, 20, interval);
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
            _handler = new AsyncIntervalFlushHandler(new SimpleBatchFactory(""), _mockRequestHandler.Object, queueSize, 20, 20000);
            await Task.Delay(100);

            for (int i = 0; i < queueSize; i++)
            {
                _ = _handler.Process(new Track(null, null, null, null));
            }

            _handler.Flush();

            _mockRequestHandler.Verify(r => r.MakeRequest(It.IsAny<Batch>()), times: Times.Exactly(5));
        }

        [Test]
        public void ProcessActionFlushWhenQueueIsFull()
        {
            var queueSize = 10;
            _handler = new AsyncIntervalFlushHandler(new SimpleBatchFactory(""), _mockRequestHandler.Object, queueSize, 20, 20000);
            
            for (int i = 0; i < queueSize; i++)
            {
                _ = _handler.Process(new Track(null, null, null, null));
            }

            _mockRequestHandler.Verify(r => r.MakeRequest(It.IsAny<Batch>()), times: Times.Exactly(1));
        }
    }
}
