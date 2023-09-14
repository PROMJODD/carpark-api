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

        public MOrganizationUser AddUserToOrganization(MOrganizationUser user)
        {
            try
            {
                user.OrgCustomId = orgId;
                context!.OrganizationUsers!.AddAsync(user);
                context.SaveChanges();
            }
            catch (Exception)
            {
                throw;
            }

            return user;
        }

        public bool IsUserNameExist(string userName)
        {
            try
            {
                var cnt = context!.OrganizationUsers!.Where(
                        p => p!.UserName!.Equals(userName) && p!.OrgCustomId!.Equals(orgId)
                    ).Count();

                return cnt >= 1;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}