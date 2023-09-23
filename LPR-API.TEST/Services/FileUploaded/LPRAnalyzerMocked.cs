using Moq;
using Moq.Protected;
using System.Net;
using Prom.LPR.Api.ExternalServices.Recognition;

namespace Prom.LPR.Test.Api.Services
{
    public class LPRAnalyzerMocked : ImageAnalyzerHttpBase
    {
        private string json = $@"
{{
    ""status"": 200,
    ""message"": ""Result"",
    ""data"": {{
      ""_Id"": ""650ed04db4cf0745d0dd4689"",
      ""license"": ""1xx2005"",
      ""province"": ""Bangkok"",
      ""vehBrand"": ""Toyota"",
      ""vehClass"": ""sedan"",
      ""vehColor"": ""Yellow"",
      ""remaining"": 99988
    }}
}}";
        public LPRAnalyzerMocked()
        {
        }

        public override string GetFormUploadKey()
        {
            return "mocked";
        }

        public override HttpClient GetHttpClient()
        {
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
                Content = new StringContent(json),
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
