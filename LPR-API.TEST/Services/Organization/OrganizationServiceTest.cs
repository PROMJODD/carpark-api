using Moq;
using Xunit;
using Prom.LPR.Api.Database;
using Prom.LPR.Api.Models;
using Prom.LPR.Api.Database.Repositories;
using Prom.LPR.Api.Services;
using Prom.LPR.Test.Database;

namespace Prom.LPR.Test.Api.Services;

public class OrganizationServiceTest
{
    private IOrganizationRepository CreateOrgRepository(string orgId, 
        List<MOrganization> lists, 
        List<MOrganizationUser> orgUsers,
        List<MUser> users)
    {
        var ctxMock = new Mock<IDataContext>();
        ctxMock.Setup(x => x.Organizations).Returns(DbContextMock.GetQueryableMockDbSet(lists));
        ctxMock.Setup(x => x.OrganizationUsers).Returns(DbContextMock.GetQueryableMockDbSet(orgUsers));
        ctxMock.Setup(x => x.Users).Returns(DbContextMock.GetQueryableMockDbSet(users));
        var repo = new OrganizationRepository(ctxMock.Object);
        repo.SetCustomOrgId(orgId);

        return repo;
    }

    private IUserRepository CreateUserRepository(string orgId, List<MUser> users)
    {
        var ctxMock = new Mock<IDataContext>();
        ctxMock.Setup(x => x.Users).Returns(DbContextMock.GetQueryableMockDbSet(users));
        var repo = new UserRepository(ctxMock.Object);
        repo.SetCustomOrgId(orgId);

        return repo;
    }

    private void CreateOrganization(List<MOrganization> list, string orgCustomId, string orgName)
    {
        var r = new MOrganization
        {
            OrgCustomId = orgCustomId,
            OrgName = orgName,
        };

        list.Add(r);
    }

    private void CreateUser(List<MUser> list, string name, string email)
    {
        var r = new MUser
        {
            UserName = name,
            UserEmail = email,
        };

        list.Add(r);
    }

    private void CreateOrgUser(List<MOrganizationUser> list, string name, string email, string orgId, string roleList)
    {
        var r = new MOrganizationUser
        {
            UserName = name,
            OrgCustomId = orgId,
            RolesList = roleList,
        };

        list.Add(r);
    }

    private void GenerateUsers(List<MUser> list, int cnt)
    {
        for (int i=1; i<=cnt; i++)
        {
            CreateUser(list, $"user-{i}", $"email-{i}");
        }
    }

    private void GenerateOrganizations(List<MOrganization> list, int cnt)
    {
        for (int i=1; i<=cnt; i++)
        {
            CreateOrganization(list, $"org-id-{i}", $"org-name-{i}");
        }
    }

    private void GenerateOrgUsers(List<MOrganizationUser> list, int cnt, string orgId)
    {
        for (int i=1; i<=cnt; i++)
        {
            CreateOrgUser(list, $"user-{i}", $"email-{i}", orgId, "");
        }
    }

    [Theory]
    [InlineData("org-id-1", 5)]
    [InlineData("org-id-2", 4)]
    [InlineData("org-id-3", 4)]
    public void GetOrganizationTest(string orgId, int loopCnt)
    {
        var orgs = new List<MOrganization>();
        GenerateOrganizations(orgs, loopCnt);

        var users = new List<MUser>();

        var orgRepo = CreateOrgRepository("", orgs, new List<MOrganizationUser>(), users);
        var userRepo = CreateUserRepository("", users);

        var userSvc = new UserService(userRepo);
        var svc = new OrganizationService(orgRepo, userSvc);

        var t = svc.GetOrganization(orgId);
        var o = t.Result;

        Assert.NotNull(t);
        Assert.NotNull(o);
        Assert.Equal(o.OrgCustomId, orgId);
    }

    [Theory]
    [InlineData("default", 0, "user-0", false)]
    [InlineData("default", 5, "user-0", false)]
    [InlineData("globalx", 4, "user-1", true)]
    [InlineData("default", 4, "user-2", true)]
    [InlineData("xxxxxxx", 4, "user-3", true)]
    public void IsUserNameExistTest(string orgId, int loopCnt, string userName, bool expectedResult)
    {
        var orgs = new List<MOrganization>();
        GenerateOrganizations(orgs, loopCnt);

        var orgUsers = new List<MOrganizationUser>();
        GenerateOrgUsers(orgUsers, loopCnt, orgId);

        var users = new List<MUser>();

        var orgRepo = CreateOrgRepository("", orgs, orgUsers, users);
        var userRepo = CreateUserRepository("", users);

        var userSvc = new UserService(userRepo);
        var svc = new OrganizationService(orgRepo, userSvc);

        var isFound = svc.IsUserNameExist(orgId, userName);

        Assert.Equal(expectedResult, isFound);
    }

    [Theory]
    [InlineData("default", 0, "user-1", "NOTFOUND")]
    [InlineData("default", 5, "user-0", "NOTFOUND")]
    [InlineData("globalx", 4, "user-1", "NOTFOUND_INORG")]
    [InlineData("default", 4, "user-2", "NOTFOUND_INORG")]
    [InlineData("xxxxxxx", 4, "user-3", "OK")]
    public void VerifyUserInOrganizationTest(string orgId, int loopCnt, string userName, string expectedResult)
    {
        var orgs = new List<MOrganization>();
        GenerateOrganizations(orgs, loopCnt);

        var orgUsers = new List<MOrganizationUser>();
        GenerateOrgUsers(orgUsers, loopCnt, orgId);
        if (orgUsers.Count >= 2)
        {
            orgUsers.RemoveAt(0); //user-1 not in org
            orgUsers.RemoveAt(0); //user-2 not in org
        }

        var users = new List<MUser>();
        GenerateUsers(users, loopCnt);

        var orgRepo = CreateOrgRepository("", orgs, orgUsers, users);
        var userRepo = CreateUserRepository("", users);

        var userSvc = new UserService(userRepo);
        var svc = new OrganizationService(orgRepo, userSvc);

        var mv = svc.VerifyUserInOrganization(orgId, userName);

        Assert.Equal(expectedResult, mv.Status);
    }

    [Theory]
    [InlineData("default", 0, "user-1", 1, "USER_NAME_NOTFOUND")]
    [InlineData("default", 5, "user-1", 0, "OK")]
    [InlineData("globalx", 4, "user-x", 1, "USER_NAME_NOTFOUND")]
    [InlineData("default", 4, "user-2", 3, "USER_DUPLICATE")]
    [InlineData("xxxxxxx", 4, "user-3", -1, "USER_ID_NOTFOUND")]
    public void AddUserToOrganizationTest(string orgId, int loopCnt, string userName, int userId, string expectedResult)
    {
        var orgs = new List<MOrganization>();
        GenerateOrganizations(orgs, loopCnt);

        var orgUsers = new List<MOrganizationUser>();
        GenerateOrgUsers(orgUsers, loopCnt, orgId);
        if (orgUsers.Count > 0)
        {
            orgUsers.RemoveAt(0);
        }

        var users = new List<MUser>();
        GenerateUsers(users, loopCnt);

        var orgRepo = CreateOrgRepository("", orgs, orgUsers, users);
        var userRepo = CreateUserRepository("", users);

        var userSvc = new UserService(userRepo);
        var svc = new OrganizationService(orgRepo, userSvc);

        var id = Guid.NewGuid().ToString();
        if (users.Count <= 0)
        {
            id = Guid.NewGuid().ToString();
        }
        else if (userId >= 0)
        {
            id = users[userId].UserId.ToString();
        }

        var ou = new MOrganizationUser() { UserName = userName, UserId = id };
        var mv = svc.AddUserToOrganization(orgId, ou);

        Assert.Equal(expectedResult, mv.Status);
    }
}
