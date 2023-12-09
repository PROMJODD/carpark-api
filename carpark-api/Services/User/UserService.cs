using Prom.LPR.Api.Models;
using Prom.LPR.Api.ModelsViews;
using Prom.LPR.Api.Database.Repositories;

namespace Prom.LPR.Api.Services
{
    public class UserService : BaseService, IUserService
    {
        private readonly IUserRepository? repository = null;

        public UserService(IUserRepository repo) : base()
        {
            repository = repo;
        }

        public MVUser? AddUser(string orgId, MUser user)
        {
            //Improvement(validation) : Added validation here

            repository!.SetCustomOrgId(orgId);
            var r = new MVUser();

            var f1 = IsEmailExist(orgId, user!.UserEmail!);
            var f2 = IsUserNameExist(orgId, user!.UserName!);

            if (f1)
            {
                r.Status = "EMAIL_DUPLICATE";
                r.Description = "Email is duplicate";

                return r;
            }

            if (f2)
            {
                r.Status = "USERNAME_DUPLICATE";
                r.Description = "User name is duplicate";

                return r;
            }

            var result = repository!.AddUser(user);

            r.Status = "OK";
            r.Description = "Success";
            r.User = result;

            return r;
        }

        public IEnumerable<MUser> GetUsers(string orgId)
        {
            repository!.SetCustomOrgId(orgId);
            var result = repository!.GetUsers();

            return result;
        }

        public bool IsEmailExist(string orgId, string email)
        {
            repository!.SetCustomOrgId(orgId);
            var result = repository!.IsEmailExist(email);

            return result;
        }

        public bool IsUserNameExist(string orgId, string userName)
        {
            repository!.SetCustomOrgId(orgId);
            var result = repository!.IsUserNameExist(userName);

            return result;
        }

        public bool IsUserIdExist(string orgId, string userId)
        {
            repository!.SetCustomOrgId(orgId);
            var result = repository!.IsUserIdExist(userId);

            return result;
        }

        public MUser GetUserByName(string orgId, string userName)
        {
            repository!.SetCustomOrgId(orgId);
            var result = repository!.GetUserByName(userName);

            return result;
        }
    }
}
