using Prom.LPR.Api.Models;
using Prom.LPR.Api.Database;
using Microsoft.EntityFrameworkCore;

namespace Prom.LPR.Api.Database.Repositories
{
    public class OrganizationRepository : BaseRepository, IOrganizationRepository
    {
        public OrganizationRepository(DataContext ctx)
        {
            context = ctx;
        }

        public Task<MOrganization> GetOrganization()
        {
            try
            {
                var result = context!.Organizations!.Where(x => x.OrgCustomId!.Equals(orgId)).FirstOrDefaultAsync();
                return result!;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}