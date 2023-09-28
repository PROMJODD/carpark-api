using Prom.LPR.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Prom.LPR.Api.Database.Repositories
{
    public class OrganizationRepository : BaseRepository, IOrganizationRepository
    {
        public OrganizationRepository(IDataContext ctx)
        {
            context = ctx;
        }

        public Task<MOrganization> GetOrganization()
        {
            var result = context!.Organizations!.Where(x => x.OrgCustomId!.Equals(orgId)).FirstOrDefaultAsync();
            return result!;
        }

        public MOrganizationUser AddUserToOrganization(MOrganizationUser user)
        {
            user.OrgCustomId = orgId;
            context!.OrganizationUsers!.Add(user);
            context.SaveChanges();

            return user;
        }

        public bool IsUserNameExist(string userName)
        {
            var cnt = context!.OrganizationUsers!.Where(
                    p => p!.UserName!.Equals(userName) && p!.OrgCustomId!.Equals(orgId)
                ).Count();

            return cnt >= 1;
        }

        public MOrganizationUser GetUserInOrganization(string userName)
        {
            var m = context!.OrganizationUsers!.Where(
                p => p!.UserName!.Equals(userName) && p!.OrgCustomId!.Equals(orgId)).FirstOrDefault();

            return m!;
        }
    }
}
