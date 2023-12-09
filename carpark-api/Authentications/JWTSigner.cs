using Microsoft.IdentityModel.Tokens;

namespace Prom.LPR.Api.Authentications
{
    public class JwtSigner : IJwtSigner
    {
        public JwtSigner()
        {
        }

        public static void ResetSigedKeyJson()
        {
            //For unit testing
            JwtSignerKey.ResetSigedKeyJson();
        }
 
        public SecurityKey GetSignedKey(string? url)
        {
            return JwtSignerKey.GetSignedKey(url);
        }
    }
}
