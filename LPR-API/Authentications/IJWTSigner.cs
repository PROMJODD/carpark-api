using Microsoft.IdentityModel.Tokens;

namespace Prom.LPR.Api.Authentications
{
    public interface IJwtSigner
    {
        public SecurityKey GetSignedKey(string? url);
        public string GetSignedKeyJson(string? url);
    }
}
