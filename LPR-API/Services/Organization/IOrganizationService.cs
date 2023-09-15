using Prom.LPR.Api.Models;
using Prom.LPR.Api.ModelsViews;

namespace Prom.LPR.Api.Services
{
    public interface IOrganizationService
    {
        public Task<MOrganization> GetOrganization(string orgId);
        public MVOrganizationUser AddUserToOrganization(string orgId, MOrganizationUser user);
        public bool IsUserNameExist(string orgId, string userName);
        public MVOrganizationUser VerifyUserInOrganization(string orgId, string userName);
    }
}
