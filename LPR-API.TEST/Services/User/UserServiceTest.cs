using Moq;
using Xunit;
using Prom.LPR.Api.Database;
using Prom.LPR.Api.Models;
using Prom.LPR.Api.Database.Repositories;
using Prom.LPR.Api.Services;
using Prom.LPR.Test.Database;

namespace Prom.LPR.Test.Api.Services;

public class UserServiceTest
{
    private IUserRepository CreateRepository(string orgId, List<MUser> lists)
    {
        var ctxMock = new Mock<IDataContext>();
        ctxMock.Setup(x => x.Users).Returns(DbContextMock.GetQueryableMockDbSet(lists));
        var repo = new UserRepository(ctxMock.Object);
        repo.SetCustomOrgId(orgId);

        return repo;
    }

    private void CreateUser(List<MUser> list, string OrgId, string name, string email)
    {
        var r = new MUser
        {
            UserName = name,
            UserEmail = email,
        };

        list.Add(r);
    }

    private void GenerateUsers(List<MUser> list, int cnt, string OrgId)
    {
        for (int i=1; i<=cnt; i++)
        {
            CreateUser(list, OrgId, $"user-{i}", $"email-{i}");
        }
    }

    [Theory]
    [InlineData("default", 5, 5)]
    [InlineData("globalx", 4, 4)]
    [InlineData("default", 4, 4)]
    public void GetUsersTest(string orgId, int loopCnt, int expectedCount)
    {
        var users = new List<MUser>();
        GenerateUsers(users, loopCnt, orgId);

        var repo = CreateRepository(orgId, users);

        var svc = new UserService(repo);
        var list = svc.GetUsers(orgId);

        Assert.Equal(expectedCount, list.Count());
    }

    [Theory]
    [InlineData("default", 0, "email-0", false)]
    [InlineData("default", 5, "email-0", false)]
    [InlineData("globalx", 4, "email-1", true)]
    [InlineData("default", 4, "email-2", true)]
    [InlineData("xxxxxxx", 4, "email-3", true)]
    public void IsEmailExist(string orgId, int loopCnt, string email, bool expectedResult)
    {
        var users = new List<MUser>();
        GenerateUsers(users, loopCnt, orgId);

        var repo = CreateRepository(orgId, users);

        var svc = new UserService(repo);
        var isFound = svc.IsEmailExist(orgId, email);

        Assert.Equal(expectedResult, isFound);
    }

    [Theory]
    [InlineData("default", 0, "user-0", false)]
    [InlineData("default", 5, "user-0", false)]
    [InlineData("globalx", 4, "user-1", true)]
    [InlineData("default", 4, "user-2", true)]
    [InlineData("xxxxxxx", 4, "user-3", true)]
    public void IsUserNameExist(string orgId, int loopCnt, string name, bool expectedResult)
    {
        var users = new List<MUser>();
        GenerateUsers(users, loopCnt, orgId);

        var repo = CreateRepository(orgId, users);

        var svc = new UserService(repo);
        var isFound = svc.IsUserNameExist(orgId, name);

        Assert.Equal(expectedResult, isFound);
    }

    [Theory]
    [InlineData("default", 5, "user-1")]
    [InlineData("globalx", 4, "user-2")]
    [InlineData("default", 4, "user-3")]
    [InlineData("default", 4, "user-4")]
    public void GetUserNameFound(string orgId, int loopCnt, string name)
    {
        var users = new List<MUser>();
        GenerateUsers(users, loopCnt, orgId);

        var repo = CreateRepository(orgId, users);

        var svc = new UserService(repo);
        var t = svc.GetUserByName(orgId, name);

        Assert.NotNull(t);
        Assert.Equal(name, t.UserName);
    }

    [Theory]
    [InlineData("default", 5, "user-0")]
    [InlineData("globalx", 4, "user-0")]
    [InlineData("default", 1, "user-0")]
    [InlineData("default", 0, "user-4")]
    public void GetUserNameNotFound(string orgId, int loopCnt, string name)
    {
        var users = new List<MUser>();
        GenerateUsers(users, loopCnt, orgId);

        var repo = CreateRepository(orgId, users);

        var svc = new UserService(repo);
        var t = svc.GetUserByName(orgId, name);

        Assert.Null(t);
    }

    [Theory]
    [InlineData("default", 0, 0, false)]
    [InlineData("globalx", 4, 3, true)]
    [InlineData("default", 4, 0, true)]
    [InlineData("xxxxxxx", 4, 1, true)]
    [InlineData("xxxxxxx", 5, -1, false)]
    public void IsUserIdExist(string orgId, int loopCnt, int userId, bool expectedResult)
    {
        var users = new List<MUser>();
        GenerateUsers(users, loopCnt, orgId);

        var id = "";        
        if (users.Count <= 0)
        {
            id = Guid.NewGuid().ToString();
        }
        else if (userId >= 0)
        {
            id = users[userId].UserId.ToString();
        }

        var repo = CreateRepository(orgId, users);

        var svc = new UserService(repo);
        var isFound = svc.IsUserIdExist(orgId, id!);

        Assert.Equal(expectedResult, isFound);
    }


    [Theory]
    [InlineData("user-1", "email-1", 5, "EMAIL_DUPLICATE")]
    [InlineData("user-1", "email-x", 5, "USERNAME_DUPLICATE")]
    [InlineData("user-x", "email-x", 5, "OK")]
    [InlineData("user-1", "email-1", 0, "OK")]
    public void AddUserTest(string user, string email, int loopCnt, string expectedResult)
    {
        var orgId = "fake-id";
        var users = new List<MUser>();
        GenerateUsers(users, loopCnt, orgId);

        var repo = CreateRepository(orgId, users);
        var u = new MUser() { UserName = user, UserEmail = email };

        var svc = new UserService(repo);
        var returnUser = svc.AddUser(orgId, u);

        Assert.Equal(expectedResult, returnUser!.Status);
    }
}
