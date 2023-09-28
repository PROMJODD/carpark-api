using Serilog;
using System.Security.Claims;
using Prom.LPR.Api.ModelsViews;
using Prom.LPR.Api.Services;

namespace Prom.LPR.Api.Authentications
{
    public class BearerAuthenticationRepo : IBearerAuthenticationRepo
    {
        private readonly IOrganizationService? service = null;

        public BearerAuthenticationRepo(IOrganizationService svc)
        {
            service = svc;
        }

        private MVOrganizationUser? VerifyUser(string orgId, string user)
        {
            //TODO : Added chaching mechanism here
            var m = service!.VerifyUserInOrganization(orgId, user);
            return m;
        }

        public User? Authenticate(string orgId, string user, string password, HttpRequest request)
        {
            var m = VerifyUser(orgId, user);
            if (m == null)
            {
                return null;
            }

            if (!m.Status!.Equals("OK"))
            {
                Log.Information(m.Description!);
                return null;
            }

            var u = new User()
            {
                UserName = user,
                Password = "",
                UserId = m.User!.UserId,
                Role = m.OrgUser!.RolesList,
                AuthenType = "JWT",
                OrgId = m.OrgUser.OrgCustomId,
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
