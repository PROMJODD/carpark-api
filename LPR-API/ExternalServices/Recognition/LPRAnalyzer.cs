using System.Net.Http.Headers;
using Prom.LPR.Api.Utils;

namespace Prom.LPR.Api.ExternalServices.Recognition
{
    public class LPRAnalyzer : ImageAnalyzerHttpBase
    {
        private string lprBaseUrl = "";
        private string lprPath = "";
        private string lprAuthKey = "";

        public LPRAnalyzer(IConfiguration cfg)
        {
            lprBaseUrl = ConfigUtils.GetConfig(cfg, "LPR:lprBaseUrl");
            lprPath = ConfigUtils.GetConfig(cfg, "LPR:lprPath");
            lprAuthKey = ConfigUtils.GetConfig(cfg, "LPR:lprAuthKey");
        }

        public override string GetFormUploadKey()
        {
            return "image";
        }

        public override HttpClient GetHttpClient()
        {
            var client = new HttpClient();
            Uri baseUri = new Uri(lprBaseUrl);
            client.BaseAddress = baseUri;
            client.Timeout = TimeSpan.FromMilliseconds(1000);

            return client;
        }

        public override HttpRequestMessage GetRequestMessage()
        {
            //Bearer Authentication
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, lprPath);
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", lprAuthKey);
            var productValue = new ProductInfoHeaderValue("lpr-api", "1.0");
            requestMessage.Headers.UserAgent.Add(productValue);

            return requestMessage;
        }
    }
}
