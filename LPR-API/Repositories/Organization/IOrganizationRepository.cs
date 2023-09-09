using Prom.LPR.Api.Models;

namespace Prom.LPR.Api.Repositories
{
    public interface IOrganizationRepository
    {
        public void SetCustomOrgId(string customOrgId);
        public Task<MOrganization> GetOrganization();
    }
}
