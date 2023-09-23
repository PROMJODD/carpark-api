using Serilog;
using Microsoft.IdentityModel.Tokens;

namespace Prom.LPR.Api.Authentications
{
    public class JWTSigner : IJWTSigner
    {
        private static string? signedKeyJson = null;

        public JWTSigner()
        {
        }

        public void ResetSigedKeyJson()
        {
            //For unit testing
            signedKeyJson = null;
        }
 
        public SecurityKey GetSignedKey(string? url)
        {
            signedKeyJson = GetSignedKeyJson(url);
            return new JsonWebKey(signedKeyJson);
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
