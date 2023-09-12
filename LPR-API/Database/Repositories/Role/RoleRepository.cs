using Prom.LPR.Api.Models;

namespace Prom.LPR.Api.Database.Repositories
{
    public class RoleRepository : BaseRepository, IRoleRepository
    {
        public RoleRepository(DataContext ctx)
        {
            context = ctx;
        }

        public IEnumerable<MRole> GetRolesList(string rolesList)
        {
            try
            {
                var list = rolesList.Split(',').ToList();
                var arr = context!.Roles!.Where(p => list.Contains(p.RoleName!)).ToList();

                return arr;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}