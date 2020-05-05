using Moq;
using NUnit.Framework;
using Segment;
using Segment.Model;
using Segment.Request;
using System;
using System.Collections.Generic;
using System.Net;

namespace Segment.Test.Request
{
    [TestFixture]
    public class BlockingRequestHandlerTests
    {
        private Mock<IHttpClient> _mockHttpClient;
        private Mock<WebHeaderCollection> _mockHeaders;
        private Client _client;

        private BlockingRequestHandler _handler;

        [SetUp]
        public void Init()
        {
            _mockHttpClient = new Mock<IHttpClient>(MockBehavior.Strict);
            _mockHeaders = new Mock<WebHeaderCollection>(MockBehavior.Strict);
            _mockHeaders.Setup(x => x.Set(It.IsAny<string>(), It.IsAny<string>())).Verifiable();

            _mockHttpClient.Setup(x => x.UploadData(It.IsAny<Uri>(), "POST", It.IsAny<byte[]>())).Returns(new byte[] { });
            _mockHttpClient.Setup(x => x.Headers).Returns(() => _mockHeaders.Object);

            _client = new Client("foo");
            _handler = new BlockingRequestHandler(_client, new TimeSpan(0, 0, 10), _mockHttpClient.Object, new Backo(max: 500, jitter: 0));
        }


        [Test]
        public void RequestIncludesRequiredHeaders()
        {
            var batch = GetBatch();

            _handler.MakeRequest(batch).GetAwaiter().GetResult();

            _mockHeaders.Verify(x => x.Set("Authorization", It.IsAny<string>()), Times.Once);
            _mockHeaders.Verify(x => x.Set("Content-Type", "application/json; charset=utf-8"), Times.Once);
        }

        [Test]
        public void RequestIncludesGzipHeaderWhenCompressRequestIsTrue()
        {
            var batch = GetBatch();
            _client.Config.SetGzip(true);

            _handler.MakeRequest(batch).GetAwaiter().GetResult();
            _mockHeaders.Verify(x => x.Set("Content-Encoding", "gzip"), Times.Once);
        }

        [Test]
        public void RequestIncludes()
        {
            _mockHeaders.Verify(x => x.Set("User-Agent", Defaults.UserAgent), Times.Once);
        }



        [Test]
        public void MakeRequestWith200StatusCode()
        {
            //Arrange
            var batch = GetBatch();

            //Act
            _handler.MakeRequest(batch).GetAwaiter().GetResult();

            //Assert
            Assert.AreEqual(1, _client.Statistics.Succeeded);
            Assert.AreEqual(0, _client.Statistics.Failed);
            AssertSendAsyncWasCalled();
        }

        [Test]
        public void MakeRequestWith5xxStatusCode()
        {
            var batch = GetBatch();
            ClientThrowWebException(HttpStatusCode.InternalServerError);

            _handler.MakeRequest(batch).GetAwaiter().GetResult();

            Assert.AreEqual(0, _client.Statistics.Succeeded);
            Assert.AreEqual(1, _client.Statistics.Failed);
            AssertSendAsyncWasCalled(4);
        }

        [Test]
        public void MakeRequestWith429StatusCode()
        {
            //Arrange
            var batch = GetBatch();
            ClientThrowWebException((HttpStatusCode)429);

            //Act            
            _handler.MakeRequest(batch).GetAwaiter().GetResult();

            //Assert
            Assert.AreEqual(0, _client.Statistics.Succeeded);
            Assert.AreEqual(1, _client.Statistics.Failed);
            AssertSendAsyncWasCalled(4);
        }


        [Test]
        public void MakeRequestWith4xxStatusCode()
        {
            //Arrange
            var batch = GetBatch();
            ClientThrowWebException(HttpStatusCode.NotFound);

            //Act
            _handler.MakeRequest(batch).GetAwaiter().GetResult();

            //Assert
            Assert.AreEqual(0, _client.Statistics.Succeeded);
            Assert.AreEqual(1, _client.Statistics.Failed);
            AssertSendAsyncWasCalled(1);
        }
        [Test]
        public void MakeRequestWithErrorStatusCodeRetryUntilSuccess()
        {
            //Arrange
            MultipleHttpResponseBehavior(
                HttpStatusCode.InternalServerError,
                HttpStatusCode.NotImplemented,
                (HttpStatusCode)429,
                new byte[] { });

            var batch = GetBatch();

            //Act
            _handler.MakeRequest(batch).GetAwaiter().GetResult();

            //Assert
            Assert.AreEqual(1, _client.Statistics.Succeeded);
            Assert.AreEqual(0, _client.Statistics.Failed);
            AssertSendAsyncWasCalled(4);
        }
        [Test]
        public void MakeRequestWithErrorStatusCodeRetryUntil4xxErrorExcept429()
        {
            //Arrange
            MultipleHttpResponseBehavior(
                HttpStatusCode.InternalServerError,
                HttpStatusCode.NotImplemented,
                (HttpStatusCode)429,
                HttpStatusCode.NotFound);

            var batch = GetBatch();

            //Act
            _handler.MakeRequest(batch).GetAwaiter().GetResult();

            //Assert
            Assert.AreEqual(0, _client.Statistics.Succeeded);
            Assert.AreEqual(1, _client.Statistics.Failed);
            AssertSendAsyncWasCalled(4);
        }


        private void ClientThrowWebException(HttpStatusCode httpStatusCode)
        {
            HttpWebResponse response = Mock.Of<HttpWebResponse>(x => x.StatusCode == httpStatusCode);
            _mockHttpClient
                .Setup(x => x.UploadData(It.IsAny<Uri>(), "POST", It.IsAny<byte[]>()))
                .Throws(new WebException("", null, WebExceptionStatus.UnknownError, response));
        }

        private void MultipleHttpResponseBehavior(params object[] results)
        {
            var seq = _mockHttpClient.SetupSequence(x => x.UploadData(It.IsAny<Uri>(), "POST", It.IsAny<byte[]>()));
            foreach (var r in results)
            {
                if (r is HttpStatusCode httpStatusCode)
                {
                    HttpWebResponse response = Mock.Of<HttpWebResponse>(x => x.StatusCode == httpStatusCode);
                    seq.Throws(new WebException("", null, WebExceptionStatus.UnknownError, response));
                }
                else if (r is byte[] wr)
                    seq.Returns(wr);
            }
        }

        private void AssertSendAsyncWasCalled(int times = 1)
        {
            _mockHttpClient
                .Verify(x => x.UploadData(It.IsAny<Uri>(), "POST", It.IsAny<byte[]>()), Times.Exactly(times));
        }

        private Batch GetBatch()
        {
            return new Batch("", new List<BaseAction> { new Track("user", "TestEvent", null, null) });
        }
    }
}
