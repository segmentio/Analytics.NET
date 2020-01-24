using Moq;
using Moq.Protected;
using NUnit.Framework;
using Segment;
using Segment.Model;
using Segment.Request;
using System;
using System.Collections.Generic;
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
        private HttpResponseMessage _httpResponse;
        private Client _client;

        private BlockingRequestHandler _handler { get; set; }


        [SetUp]
        public void Init()
        {
            _mockHttpMessageHandler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            _httpResponse = new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.OK
            };

            _mockHttpMessageHandler.Protected().Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
                )
                // prepare the expected response of the mocked http call
            .ReturnsAsync(() => _httpResponse)
            .Verifiable();

            _client = new Client("foo");
            _handler = new BlockingRequestHandler(_client, new TimeSpan(0, 0, 10), new HttpClient(_mockHttpMessageHandler.Object));
        }

        [Test]
        public async Task MakeRequestWith5xxStatusCode()
        {
            //Arrange
            _httpResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.InternalServerError
            };

            var batch = GetBatch();

            //Act
            await _handler.MakeRequest(batch);

            //Assert
            Assert.AreEqual(0, _client.Statistics.Succeeded);
            Assert.AreEqual(1, _client.Statistics.Failed);
            AssertSendAsyncWasCalled(7);
        }

        [Test]
        public async Task MakeRequestWith429StatusCode()
        {
            //Arrange
            _httpResponse = new HttpResponseMessage
            {
                StatusCode = (HttpStatusCode)429
            };

            var batch = GetBatch();

            //Act
            await _handler.MakeRequest(batch);

            //Assert
            Assert.AreEqual(0, _client.Statistics.Succeeded);
            Assert.AreEqual(1, _client.Statistics.Failed);
            AssertSendAsyncWasCalled(7);
        }


        [Test]
        public async Task MakeRequestWith4xxStatusCode()
        {
            //Arrange
            _httpResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.MethodNotAllowed
            };

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
