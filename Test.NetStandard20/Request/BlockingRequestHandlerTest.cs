using Moq;
using Moq.Protected;
using NUnit.Framework;
using Segment;
using Segment.Model;
using Segment.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Test.NetStandard20.Request
{

    [TestFixture()]
    class BlockingRequestHandlerTest
    {
        private Mock<HttpMessageHandler> _mockHttpMessageHandler;
        private Client _client;

        private BlockingRequestHandler _handler;
        Func<HttpResponseMessage> _httpBehavior;

        [SetUp]
        public void Init()
        {
            _mockHttpMessageHandler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            _httpBehavior = SingleHttpResponseBehavior(HttpStatusCode.OK);

            _mockHttpMessageHandler.Protected().Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
                )
            // prepare the expected response of the mocked http call
            .ReturnsAsync(() => _httpBehavior())
            .Verifiable();

            _client = new Client("foo");
            _handler = new BlockingRequestHandler(_client, new TimeSpan(0, 0, 10), new HttpClient(_mockHttpMessageHandler.Object), 500);
        }

        [Test]
        public async Task MakeRequestWith5xxStatusCode()
        {
            //Arrange
            _httpBehavior = SingleHttpResponseBehavior(HttpStatusCode.InternalServerError);
            var batch = GetBatch();

            //Act
            await _handler.MakeRequest(batch);

            //Assert
            Assert.AreEqual(0, _client.Statistics.Succeeded);
            Assert.AreEqual(1, _client.Statistics.Failed);
            AssertSendAsyncWasCalled(4);
        }

        [Test]
        public async Task MakeRequestWith429StatusCode()
        {
            //Arrange
            _httpBehavior = SingleHttpResponseBehavior((HttpStatusCode)429);
            var batch = GetBatch();

            //Act
            await _handler.MakeRequest(batch);

            //Assert
            Assert.AreEqual(0, _client.Statistics.Succeeded);
            Assert.AreEqual(1, _client.Statistics.Failed);
            AssertSendAsyncWasCalled(4);
        }


        [Test]
        public async Task MakeRequestWith4xxStatusCode()
        {
            //Arrange
            _httpBehavior = SingleHttpResponseBehavior(HttpStatusCode.MethodNotAllowed);
            var batch = GetBatch();

            //Act
            await _handler.MakeRequest(batch);

            //Assert
            Assert.AreEqual(0, _client.Statistics.Succeeded);
            Assert.AreEqual(1, _client.Statistics.Failed);
            AssertSendAsyncWasCalled(1);
        }

        [Test]
        public async Task MakeRequestWith200StatusCode()
        {
            //Arrange
            var batch = GetBatch();

            //Act
            await _handler.MakeRequest(batch);

            //Assert
            Assert.AreEqual(1, _client.Statistics.Succeeded);
            Assert.AreEqual(0, _client.Statistics.Failed);
            AssertSendAsyncWasCalled();
        }

        [Test]
        public async Task MakeRequestWithErrorStatusCodeRetryUntilSuccess()
        {
            //Arrange
            _httpBehavior = MultipleHttpResponseBehavior(
                HttpStatusCode.InternalServerError,
                HttpStatusCode.NotImplemented,
                (HttpStatusCode)429,
                HttpStatusCode.OK);

            var batch = GetBatch();

            //Act
            await _handler.MakeRequest(batch);

            //Assert
            Assert.AreEqual(1, _client.Statistics.Succeeded);
            Assert.AreEqual(0, _client.Statistics.Failed);
            AssertSendAsyncWasCalled(4);
        }
        [Test]
        public async Task MakeRequestWithErrorStatusCodeRetryUntil4xxErrorExcept429()
        {
            //Arrange
            _httpBehavior = MultipleHttpResponseBehavior(
                HttpStatusCode.InternalServerError,
                HttpStatusCode.NotImplemented,
                (HttpStatusCode)429,
                HttpStatusCode.NotFound);

            var batch = GetBatch();

            //Act
            await _handler.MakeRequest(batch);

            //Assert
            Assert.AreEqual(0, _client.Statistics.Succeeded);
            Assert.AreEqual(1, _client.Statistics.Failed);
            AssertSendAsyncWasCalled(4);
        }


        private Func<HttpResponseMessage> SingleHttpResponseBehavior(HttpStatusCode statusCode)
        {
            return () => new HttpResponseMessage(statusCode: statusCode);
        }

        private Func<HttpResponseMessage> MultipleHttpResponseBehavior(params HttpStatusCode[] statusCode)
        {
            var response = new Queue<HttpResponseMessage>(statusCode.Select(s => new HttpResponseMessage { StatusCode = s }));
            return () => response.Count > 0 ? response.Dequeue() : null;
        }


        private void AssertSendAsyncWasCalled(int times = 1)
        {
            _mockHttpMessageHandler.Protected().Verify("SendAsync", Times.Exactly(times),
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>());
        }

        private Batch GetBatch()
        {
            return new Batch("", new List<BaseAction> { new Track("user", "TestEvent", null, null) });
        }
    }
}
