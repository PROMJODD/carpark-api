using Prom.LPR.Api.Models;

namespace Prom.LPR.Api.Services
{
    public interface IRoleService
    {
        public IEnumerable<MRole> GetRolesList(string orgId, string rolesList);
    }
}
