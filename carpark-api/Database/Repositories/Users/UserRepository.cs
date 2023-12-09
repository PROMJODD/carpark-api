using Prom.LPR.Api.Models;

namespace Prom.LPR.Api.Database.Repositories
{
    public class UserRepository : BaseRepository, IUserRepository
    {
        public UserRepository(IDataContext ctx)
        {
            context = ctx;
        }

        public MUser AddUser(MUser user)
        {
            context!.Users!.Add(user);
            context.SaveChanges();

            return user;
        }

        public IEnumerable<MUser> GetUsers()
        {
            //Get All, do this query below will be easier for mocked unit testing
            var arr = context!.Users!.Where(p => !p.UserName!.Equals(null)).ToList();
            return arr;
        }

        public bool IsEmailExist(string email)
        {
            var cnt = context!.Users!.Where(p => p!.UserEmail!.Equals(email)).Count();
            return cnt >= 1;
        }

        public bool IsUserNameExist(string userName)
        {
            var cnt = context!.Users!.Where(p => p!.UserName!.Equals(userName)).Count();
            return cnt >= 1;
        }

        public bool IsUserIdExist(string userId)
        {
            try
            {
                Guid id = Guid.Parse(userId);
                var cnt = context!.Users!.Where(p => p!.UserId!.Equals(id)).Count();
                return cnt >= 1;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public MUser GetUserByName(string userName)
        {
            var u = context!.Users!.Where(p => p!.UserName!.Equals(userName)).FirstOrDefault();
            return u!;
        }
    }
}