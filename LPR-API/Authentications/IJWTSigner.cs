using Microsoft.IdentityModel.Tokens;

namespace Prom.LPR.Api.Authentications
{
    public interface IJWTSigner
    {
        public SecurityKey GetSignedKey(string? url);
        public string GetSignedKeyJson(string? url);
    }
}
