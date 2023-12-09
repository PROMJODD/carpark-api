using Moq;
using Xunit;
using Prom.LPR.Api.Database;
using Prom.LPR.Api.Models;
using Prom.LPR.Api.Database.Repositories;
using Prom.LPR.Api.Services;
using Prom.LPR.Test.Database;

namespace Prom.LPR.Test.Api.Services;

public class RoleServiceTest
{
    private IRoleRepository CreateRepository(string orgId, List<MRole> lists)
    {
        var ctxMock = new Mock<IDataContext>();
        ctxMock.Setup(x => x.Roles).Returns(DbContextMock.GetQueryableMockDbSet(lists));
        var repo = new RoleRepository(ctxMock.Object);
        repo.SetCustomOrgId(orgId);

        return repo;
    }

    private void CreateRole(List<MRole> list, string OrgId, string name, string roleDef)
    {
        var r = new MRole
        {
            RoleName = name,
            RoleDefinition = roleDef,
        };

        list.Add(r);
    }

    private void GenerateRoles(List<MRole> list, int cnt, string OrgId)
    {
        for (int i=1; i<=cnt; i++)
        {
            CreateRole(list, OrgId, $"role-{i}", $"def-{i}");
        }

        CreateRole(list, "xxswww", $"role-{cnt}", $"def-{cnt}");
    }

    [Theory]
    [InlineData("default", "role-1,role-2", 5, 2)]
    [InlineData("default", "role-1", 4, 1)]
    [InlineData("default", "role-x", 4, 0)]
    public void GetRolesListTest(string orgId, string roleList, int loopCnt, int expectedCount)
    {
        var roles = new List<MRole>();
        GenerateRoles(roles, loopCnt, orgId);

        var repo = CreateRepository(orgId, roles);

        var svc = new RoleService(repo);
        var list = svc.GetRolesList(orgId, roleList);

        Assert.Equal(expectedCount, list.Count());
    }
}
