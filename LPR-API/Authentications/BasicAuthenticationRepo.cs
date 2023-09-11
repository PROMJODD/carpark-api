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

        public User? Authenticate(string orgId, string user, string password)
        {
            var m = service!.VerifyApiKey(orgId, password);
            if (!m.Status!.Equals("OK"))
            {
                return null;
            }

            var u = new User()
            {
                UserName = user,
                Password = password
            };

            return u;
        }
    }
}
