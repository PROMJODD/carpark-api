using System.Security.Claims;
using Prom.LPR.Api.ModelsViews;
using Prom.LPR.Api.Services;

namespace Prom.LPR.Api.Authentications
{
    public class BasicAuthenticationRepo : IBasicAuthenticationRepo
    {
        private readonly IApiKeyService? service = null;

        public BasicAuthenticationRepo(IApiKeyService svc)
        {
            service = svc;
        }

        private MVApiKey? VerifyKey(string orgId, string password)
        {
            //Improvement(caching) : Added chaching mechanism here

            var m = service!.VerifyApiKey(orgId, password);
            if (m != null && m.Status!.Equals("OK"))
            {
                return m;
            }

            return null;
        }

        public User? Authenticate(string orgId, string user, string password, HttpRequest request)
        {
            var m = VerifyKey(orgId, password);
            if (m == null)
            {
                return null;
            }

            var u = new User()
            {
                UserName = user,
                Password = m.ApiKey!.ApiKey,
                UserId = m.ApiKey.KeyId,
                Role = m.ApiKey.RolesList,
                AuthenType = "API-KEY",
                OrgId = m.ApiKey.OrgId,
            };

            u.Claims = new[] {
                new Claim(ClaimTypes.NameIdentifier, u.UserId.ToString()!),
                new Claim(ClaimTypes.Name, user),
                new Claim(ClaimTypes.Role, u.Role!),
                new Claim(ClaimTypes.AuthenticationMethod, u.AuthenType!),
                new Claim(ClaimTypes.Uri, request.Path),
                new Claim(ClaimTypes.GroupSid, u.OrgId!),
            };

            return u;
        }
    }
}
