using Prom.LPR.Api.Models;
using Prom.LPR.Api.Database.Repositories;
using Prom.LPR.Api.ModelsViews;

namespace Prom.LPR.Api.Services
{
    public class OrganizationService : BaseService, IOrganizationService
    {
        private readonly IOrganizationRepository? repository = null;
        private readonly IUserService userService;

        public OrganizationService(IOrganizationRepository repo, IUserService userSvc) : base()
        {
            repository = repo;
            userService = userSvc;
        }

        public Task<MOrganization> GetOrganization(string orgId)
        {
            repository!.SetCustomOrgId(orgId);
            var result = repository!.GetOrganization();

            return result;
        }

        public bool IsUserNameExist(string orgId, string userName)
        {
            repository!.SetCustomOrgId(orgId);
            var result = repository!.IsUserNameExist(userName);

            return result;
        }

        public MVOrganizationUser AddUserToOrganization(string orgId, MOrganizationUser user)
        {
            //TODO(validation) : Added validation here

            repository!.SetCustomOrgId(orgId);
            var r = new MVOrganizationUser();

            var f1 = userService.IsUserNameExist(orgId, user!.UserName!);
            if (!f1)
            {
                r.Status = "USER_NAME_NOTFOUND";
                r.Description = $"User name not found [{user.UserName}] !!!";

                return r;
            }

            var f2 = userService.IsUserIdExist(orgId, user!.UserId!);
            if (!f2)
            {
                r.Status = "USER_ID_NOTFOUND";
                r.Description = $"User ID not found [{user.UserId}] !!!";

                return r;
            }

            var f3 = IsUserNameExist(orgId, user!.UserName!);
            if (f3)
            {
                r.Status = "USER_DUPLICATE";
                r.Description = $"User [{user.UserName}] already in organization !!!";

                return r;
            }

            var result = repository!.AddUserToOrganization(user);

            r.Status = "OK";
            r.Description = "Success";
            r.OrgUser = result;

            return r;
        }

        public MVOrganizationUser VerifyUserInOrganization(string orgId, string userName)
        {
            repository!.SetCustomOrgId(orgId);

            var u = userService.GetUserByName(orgId, userName);
            if (u == null)
            {
                var o = new MVOrganizationUser() 
                {
                    Status = "NOTFOUND",
                    Description = $"User [{userName}] not found !!!"
                };

                return o;
            }

            var m = repository!.GetUserInOrganization(userName);
            if (m == null)
            {
                var o = new MVOrganizationUser() 
                {
                    Status = "NOTFOUND_INORG",
                    Description = $"User [{userName}] has not been added to the organization [{orgId}] !!!",
                };

                return o;
            }

            var mv = new MVOrganizationUser() 
            {
                User = u,
                OrgUser = m,
                Status = "OK",
                Description = "Success",
            };

            return mv;
        }
    }
}