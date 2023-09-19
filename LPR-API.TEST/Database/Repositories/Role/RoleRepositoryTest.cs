using Moq;
using Xunit;
using Prom.LPR.Api.Database;
using Prom.LPR.Api.Models;
using Prom.LPR.Api.Database.Repositories;

namespace Prom.LPR.Test.Database.Repositories;

public class RoleRepositoryTest
{
    private void CreateRole(List<MRole> list, string name, string def)
    {
        var r = new MRole
        {
            RoleName = name,
            RoleDefinition = def
        };

        list.Add(r);
    }

    [Theory]
    [InlineData("VIEWER,EDITOR,OWNER", 3)]
    [InlineData("OWNER", 1)]
    [InlineData("", 0)]
    public void GetRolesListTest(string rolesList, int returnCnt)
    {
        var roles = new List<MRole>();
        CreateRole(roles, "OWNER", "xxxxxx");
        CreateRole(roles, "EDITOR", "xxxxxx");
        CreateRole(roles, "VIEWER", "xxxxxx");

        var ctxMock = new Mock<IDataContext>();
        ctxMock.Setup(x => x.Roles).Returns(DbContextMock.GetQueryableMockDbSet(roles));

        var repo = new RoleRepository(ctxMock.Object);

        var list = repo.GetRolesList(rolesList);
        var cnt = list.Count();

        Assert.Equal(returnCnt, cnt);
    }

    [Theory]
    [InlineData("VIEWER,EDITOR,OWNER", 0)]
    [InlineData("OWNER", 0)]
    [InlineData("", 0)]
    public void GetRolesNotFund(string rolesList, int returnCnt)
    {
        var roles = new List<MRole>();

        var ctxMock = new Mock<IDataContext>();
        ctxMock.Setup(x => x.Roles).Returns(DbContextMock.GetQueryableMockDbSet(roles));

        var repo = new RoleRepository(ctxMock.Object);

        var list = repo.GetRolesList(rolesList);
        var cnt = list.Count();

        Assert.Equal(returnCnt, cnt);
    }

    [Fact]
    public void GetRolesListNullParam()
    {
        var roles = new List<MRole>();

        var ctxMock = new Mock<IDataContext>();
        ctxMock.Setup(x => x.Roles).Returns(DbContextMock.GetQueryableMockDbSet(roles));

        var repo = new RoleRepository(ctxMock.Object);

        Action act = () => repo.GetRolesList(null!);
        var ex = Record.Exception(act);
        Assert.NotNull(ex);
    }
}
