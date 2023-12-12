using Prom.LPR.Api.Models;

namespace Prom.LPR.Api.Database.Repositories
{
    public interface IOrganizationRepository
    {
        public void SetCustomOrgId(string customOrgId);
        public Task<MOrganization> GetOrganization();
        public MOrganizationUser AddUserToOrganization(MOrganizationUser user);
        public bool IsUserNameExist(string userName);
        public bool IsCustomOrgIdExist(string orgCustomId);
        public MOrganizationUser GetUserInOrganization(string userName);
        public MOrganization AddOrganization(MOrganization org);
        public IEnumerable<MOrganizationUser> GetUserAllowedOrganization(string userName);
    }
}
