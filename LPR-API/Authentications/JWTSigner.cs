using Serilog;

namespace Prom.LPR.Api.Authentications
{
    public class JWTSigner
    {
        private static string? signedKeyJson = null;

        public JWTSigner()
        {
        }

        public string GetSignedKeyJson(string? url)
        {
            if (signedKeyJson != null)
            {
                return signedKeyJson;
            }

            Log.Information($"Getting JSON public key from [{url}]");

            var handler = new HttpClientHandler() 
            { 
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };

            var client = new HttpClient(handler)
            {
                Timeout = TimeSpan.FromMinutes(0.05)
            };

            var task = client.GetAsync(url);
            var response = task.Result;
            signedKeyJson = response.Content.ReadAsStringAsync().Result;

            return signedKeyJson;
        }
    }
}
