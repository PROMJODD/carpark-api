using Prom.LPR.Api.Models;

namespace Prom.LPR.Api.Database.Repositories
{
    public interface IUserRepository
    {
        public void SetCustomOrgId(string customOrgId);
        public MUser AddUser(MUser user);
        public IEnumerable<MUser> GetUsers();

        public bool IsEmailExist(string email);
        public bool IsUserNameExist(string userName);
    }
}
