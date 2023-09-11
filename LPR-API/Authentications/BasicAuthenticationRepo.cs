using Prom.LPR.Api.ModelsViews;
using Prom.LPR.Api.Services;

namespace Prom.LPR.Api.Authentications
{
    public class BasicAuthenticationRepo : IBasicAuthenticationRepo
    {
        private static IApiKeyService? service = null;

        public static void SetConfiguration(IApiKeyService svc)
        {
            service = svc;
        }

        public BasicAuthenticationRepo(IApiKeyService svc)
        {
            service = svc;
        }

        private MVApiKey? VerifyKey(string orgId, string user, string password)
        {
            //TODO : Added chaching mechanism here

            var m = service!.VerifyApiKey(orgId, password);
            if (m != null && m.Status!.Equals("OK"))
            {
                return m;
            }

            var g = service!.VerifyApiKey("global", password);
            if (g != null && g.Status!.Equals("OK"))
            {
                return g;
            }

            return null;
        }

        public User? Authenticate(string orgId, string user, string password)
        {
            var m = VerifyKey(orgId, user, password);
            if (m == null)
            {
                return null;
            }

            if (!m.Status!.Equals("OK"))
            {
                return null;
            }

            var u = new User()
            {
                UserName = user,
                Password = m.ApiKey!.ApiKey,
                UserId = m.ApiKey.KeyId,
                AuthenType = "API-KEY"
            };
Console.WriteLine($"##### [{u.UserId}] #####");
            return u;
        }
    }
}
