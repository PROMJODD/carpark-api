using Moq;
using Moq.Protected;
using System.Net;
using Prom.LPR.Api.ExternalServices.Recognition;
using System.Net.Mime;

namespace Prom.LPR.Test.Api.ExternalServices.Recognition
{
    public class LPRAnalyzerMocked : ImageAnalyzerHttpBase
    {
        private bool isReturnEmpty = false;

        public LPRAnalyzerMocked(bool returnEmpty)
        {
            isReturnEmpty = returnEmpty;
        }

        public LPRAnalyzerMocked()
        {
        }

        public override string GetFormUploadKey()
        {
            return "mocked";
        }

        public override HttpClient GetHttpClient()
        {
            var content = new StringContent("");
            if (!isReturnEmpty)
            {
                content = new StringContent("{\"field\":\"value\"}");
            }

            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handlerMock
            .Protected()
            // Setup the PROTECTED method to mock
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            // prepare the expected response of the mocked http call
            .ReturnsAsync(new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.OK,
                Content = content,
            })
            .Verifiable();

            // use real http client with mocked handler here
            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri("http://test.com/"),
            };

            return httpClient;
        }

        public override HttpRequestMessage GetRequestMessage()
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, "http://localhost");
            return requestMessage;
        }
    }
}
