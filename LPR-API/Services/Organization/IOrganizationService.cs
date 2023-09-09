using Prom.LPR.Api.Models;

namespace Prom.LPR.Api.Services
{
    public interface IOrganizationService
    {
        public Task<MOrganization> GetOrganization(string orgId);
    }
}
