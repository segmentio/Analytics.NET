using Moq;
using NUnit.Framework;
using Segment;
using Segment.Model;
using Segment.Request;
using System;
using System.Collections.Generic;
using System.Net;

namespace Test.Net35.Request
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
            _handler = new BlockingRequestHandler(_client, new TimeSpan(0, 0, 10), _mockHttpClient.Object);
        }


        [Test]
        public void RequestIncludesRequiredHeaders() {
            var batch = GetBatch();

            _handler.MakeRequest(batch).GetAwaiter().GetResult();

            _mockHeaders.Verify(x => x.Set("Authorization", It.IsAny<string>()), Times.Once);
            _mockHeaders.Verify(x => x.Set("Content-Type", "application/json; charset=utf-8"), Times.Once);
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
            HttpWebResponse response = Mock.Of<HttpWebResponse>(x => x.StatusCode == HttpStatusCode.InternalServerError);
            _mockHttpClient
                .Setup(x => x.UploadData(It.IsAny<Uri>(), "POST", It.IsAny<byte[]>()))
                .Throws(new WebException("", null, WebExceptionStatus.UnknownError, response));

            _handler.MakeRequest(batch).GetAwaiter().GetResult();

            Assert.AreEqual(0, _client.Statistics.Succeeded);
            Assert.AreEqual(1, _client.Statistics.Failed);
            AssertSendAsyncWasCalled(7);
        }
        [Test]
        public void MakeRequestWith4xxStatusCode()
        {
            //Arrange
            HttpWebResponse response = Mock.Of<HttpWebResponse>(x => x.StatusCode == HttpStatusCode.NotFound);
            _mockHttpClient
                .Setup(x => x.UploadData(It.IsAny<Uri>(), "POST", It.IsAny<byte[]>()))
                .Throws(new WebException("", null, WebExceptionStatus.UnknownError, response));
            var batch = GetBatch();

            //Act
            _handler.MakeRequest(batch).GetAwaiter().GetResult();

            //Assert
            Assert.AreEqual(0, _client.Statistics.Succeeded);
            Assert.AreEqual(1, _client.Statistics.Failed);
            AssertSendAsyncWasCalled(1);
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
