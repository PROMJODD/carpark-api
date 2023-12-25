using Moq;
using Xunit;
using Prom.LPR.Api.Database;
using Prom.LPR.Api.Models;
using Prom.LPR.Api.Database.Repositories;

namespace Prom.LPR.Test.Database.Repositories;

public class OrganizationRepositoryTest
{
    private void CreateOrg(List<MOrganization> list, string name, string customId)
    {
        var r = new MOrganization
        {
            OrgName = name,
            OrgCustomId = customId
        };

        list.Add(r);
    }

    private void CreateOrgUser(List<MOrganizationUser> list, string userName, string orgCustomId)
    {
        var r = new MOrganizationUser
        {
            UserName = userName,
            OrgCustomId = orgCustomId
        };

        list.Add(r);
    }

    [Theory]
    [InlineData("default", true)]
    [InlineData("global", true)]
    [InlineData("fake", false)]
    [InlineData("", false)]
    public void GetOrganizationTest(string name, bool found)
    {
        var orgs = new List<MOrganization>();
        CreateOrg(orgs, "default", "default");
        CreateOrg(orgs, "global", "global");

        var ctxMock = new Mock<IDataContext>();
        ctxMock.Setup(x => x.Organizations).Returns(DbContextMock.GetQueryableMockDbSet(orgs));

        var repo = new OrganizationRepository(ctxMock.Object);
        repo.SetCustomOrgId(name);

        var t = repo.GetOrganization();
        var o = t.Result;
        var isFound = o != null;

        Assert.Equal(found, isFound);
    }

    [Theory]
    [InlineData("default")]
    [InlineData("global")]
    [InlineData("fake")]
    [InlineData("")]
    public void GetOrganizationWithException(string name)
    {
        var orgs = new List<MOrganization>();
        CreateOrg(orgs, "default", null!);
        CreateOrg(orgs, "global", null!);

        var ctxMock = new Mock<IDataContext>();
        ctxMock.Setup(x => x.Organizations).Returns(DbContextMock.GetQueryableMockDbSet(orgs));

        var repo = new OrganizationRepository(ctxMock.Object);
        repo.SetCustomOrgId(name);

        Action act = () => repo.GetOrganization();
        var ex = Record.Exception(act);
        Assert.NotNull(ex);
    }

    //#### IsUserNameExist ####
    [Theory]
    [InlineData("user1", "default", true)]
    [InlineData("user2", "global", true)]
    [InlineData("user3", "fake", false)]
    [InlineData("", "", false)]
    [InlineData("", "default", false)]
    [InlineData("user1", "", false)]
    public void IsUserNameExistTest(string userName, string customOrgId, bool isExist)
    {
        var orgUsers = new List<MOrganizationUser>();
        CreateOrgUser(orgUsers, "user1", "default");
        CreateOrgUser(orgUsers, "user2", "global");

        var ctxMock = new Mock<IDataContext>();
        ctxMock.Setup(x => x.OrganizationUsers).Returns(DbContextMock.GetQueryableMockDbSet(orgUsers));

        var repo = new OrganizationRepository(ctxMock.Object);
        repo.SetCustomOrgId(customOrgId);

        var found = repo.IsUserNameExist(userName);

        Assert.Equal(isExist, found);
    }

    [Theory]
    [InlineData("user1", "default")]
    [InlineData("user2", "global")]
    public void IsUserNameExistWithException(string userName, string customOrgId)
    {
        var orgUsers = new List<MOrganizationUser>();
        CreateOrgUser(orgUsers, "user1", null!);
        CreateOrgUser(orgUsers, "user2", null!);

        var ctxMock = new Mock<IDataContext>();
        ctxMock.Setup(x => x.OrganizationUsers).Returns(DbContextMock.GetQueryableMockDbSet(orgUsers));

        var repo = new OrganizationRepository(ctxMock.Object);
        repo.SetCustomOrgId(customOrgId);

        Action act = () => repo.IsUserNameExist(userName);
        var ex = Record.Exception(act);
        Assert.NotNull(ex);
    }

    //#### AddUserToOrganization ####
    [Fact]
    public void AddUserToOrganizationSuccess()
    {
        var orgUsers = new List<MOrganizationUser>();
        CreateOrgUser(orgUsers, "user1", "default");
        CreateOrgUser(orgUsers, "user2", "global");

        var ctxMock = new Mock<IDataContext>();
        ctxMock.Setup(x => x.OrganizationUsers).Returns(DbContextMock.GetQueryableMockDbSet(orgUsers));

        var repo = new OrganizationRepository(ctxMock.Object);

        var u = new MOrganizationUser() { UserName = "fake", OrgCustomId = "fake" };
        var retUser = repo.AddUserToOrganization(u);
        var cnt = orgUsers.Count();

        Assert.Equal(3, cnt);
        Assert.Equal(u.UserName, retUser.UserName);
    }

    [Fact]
    public void AddUserToOrganizationWithException()
    {
        var orgUsers = new List<MOrganizationUser>();

        var ctxMock = new Mock<IDataContext>();
        ctxMock.Setup(x => x.OrganizationUsers).Returns(DbContextMock.GetQueryableMockDbSet(orgUsers));

        var repo = new OrganizationRepository(ctxMock.Object);

        Action act = () => repo.AddUserToOrganization(null!);
        var ex = Record.Exception(act);
        Assert.NotNull(ex);
    }


    //#### GetUserInOrganization ####
    [Theory]
    [InlineData("user1", "default")]
    [InlineData("user2", "global")]
    public void GetUserInOrganizationTest(string userName, string customOrgId)
    {
        var orgUsers = new List<MOrganizationUser>();
        CreateOrgUser(orgUsers, "user1", "default");
        CreateOrgUser(orgUsers, "user2", "global");

        var ctxMock = new Mock<IDataContext>();
        ctxMock.Setup(x => x.OrganizationUsers).Returns(DbContextMock.GetQueryableMockDbSet(orgUsers));

        var repo = new OrganizationRepository(ctxMock.Object);
        repo.SetCustomOrgId(customOrgId);

        var u = repo.GetUserInOrganization(userName);

        Assert.Equal(userName, u.UserName);
        Assert.Equal(customOrgId, u.OrgCustomId);
    }

    [Fact]
    public void GetUserInOrganizationWithException()
    {
        var orgUsers = new List<MOrganizationUser>();
        CreateOrgUser(orgUsers, "user1", null!);
        CreateOrgUser(orgUsers, "user2", null!);

        var ctxMock = new Mock<IDataContext>();
        ctxMock.Setup(x => x.OrganizationUsers).Returns(DbContextMock.GetQueryableMockDbSet(orgUsers));

        var repo = new OrganizationRepository(ctxMock.Object);
        repo.SetCustomOrgId("fake-org");

        Action act = () => repo.GetUserInOrganization("user1");
        var ex = Record.Exception(act);
        Assert.NotNull(ex);
    }

    [Theory]
    [InlineData("default", false)]
    [InlineData("abcde", true)]
    [InlineData("global", true)]
    public void IsOrganizationExistTest(string customOrgId, bool isExist)
    {
        var orgs = new List<MOrganization>();
        CreateOrg(orgs, "abcde", "abcde");
        CreateOrg(orgs, "global", "global");

        var ctxMock = new Mock<IDataContext>();
        ctxMock.Setup(x => x.Organizations).Returns(DbContextMock.GetQueryableMockDbSet(orgs));

        var repo = new OrganizationRepository(ctxMock.Object);
        repo.SetCustomOrgId(customOrgId);

        var found = repo.IsCustomOrgIdExist(customOrgId);

        Assert.Equal(isExist, found);
    }

    [Theory]
    [InlineData("abcdef")]
    public void AddOrganizationSuccess(string customOrgId)
    {
        var orgs = new List<MOrganization>();

        var ctxMock = new Mock<IDataContext>();
        ctxMock.Setup(x => x.Organizations).Returns(DbContextMock.GetQueryableMockDbSet(orgs));

        var repo = new OrganizationRepository(ctxMock.Object);
        repo.SetCustomOrgId("global");

        var o = new MOrganization() 
        { 
            OrgName = customOrgId, 
            OrgCustomId = customOrgId,
            OrgDescription = "",
        };
        var retOrg = repo.AddOrganization(o);
        var cnt = orgs.Count();

        Assert.Equal(1, cnt);
        Assert.Equal(o.OrgCustomId, retOrg.OrgCustomId);
    }


    //#### GetUserAllowedOrganization ####
    [Theory]
    [InlineData("user3", 2)]
    [InlineData("user1", 1)]
    [InlineData("user5", 0)]
    public void GetUserAllowedOrganizationTest(string userName, int orgCnt)
    {
        var orgUsers = new List<MOrganizationUser>();
        CreateOrgUser(orgUsers, "user1", "aaaaa");
        CreateOrgUser(orgUsers, "user2", "ccccc");
        CreateOrgUser(orgUsers, "user3", "bbbbb");
        CreateOrgUser(orgUsers, "user3", "ccccc");

        var ctxMock = new Mock<IDataContext>();
        ctxMock.Setup(x => x.OrganizationUsers).Returns(DbContextMock.GetQueryableMockDbSet(orgUsers));

        var repo = new OrganizationRepository(ctxMock.Object);
        repo.SetCustomOrgId("dummy");

        var arr = repo.GetUserAllowedOrganization(userName);

        Assert.Equal(orgCnt, arr.Count());
    }
}
