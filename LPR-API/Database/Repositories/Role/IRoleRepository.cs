using Prom.LPR.Api.Models;

namespace Prom.LPR.Api.Database.Repositories
{
    public interface IRoleRepository
    {
        public void SetCustomOrgId(string customOrgId);
        public IEnumerable<MRole> GetRolesList(string rolesList);
    }
}
