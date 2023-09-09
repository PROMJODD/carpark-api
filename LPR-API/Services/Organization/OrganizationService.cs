using Prom.LPR.Api.Models;
using Prom.LPR.Api.Repositories;

namespace Prom.LPR.Api.Services
{
    public class OrganizationService : BaseService, IOrganizationService
    {
        private IOrganizationRepository? repository = null;

        public OrganizationService(IOrganizationRepository repo) : base()
        {
            repository = repo;
        }

        public Task<MOrganization> GetOrganization(string orgId)
        {
            repository!.SetCustomOrgId(orgId);
            var result = repository!.GetOrganization();

            return result;
        }
    }
}