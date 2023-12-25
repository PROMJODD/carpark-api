using Moq;
using Xunit;
using Prom.LPR.Api.Services;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Prom.LPR.Api.Authorizations;
using Microsoft.AspNetCore.Authorization;
using Prom.LPR.Api.Models;

namespace Prom.LPR.Test.Api.Authorizations;

public class GenericRbacHandlerTest
{
    private List<MRole> ownerRole = new List<MRole>() { new MRole() { RoleName = "OWNER", RoleDefinition = ".+:.+" } };
    private List<MRole> viewerRole = new List<MRole>() { new MRole() { RoleName = "VIEWER", RoleDefinition = ".+:Get.+" } };
    private List<MRole> annonymousRole = new List<MRole>() { new MRole() { RoleName = "NOTHING", RoleDefinition = ".+:Xet.+" } };

    private Claim? GetClaim(string type, IEnumerable<Claim> claims)
    {
        var claim = claims.FirstOrDefault(x => x.Type == type);
        return claim;
    }

    private void AppendClaim(List<Claim> claims, string type, string value)
    {
        if (!value.Equals(""))
        {
            claims.Add(new Claim(type, value));
        }
    }

    [Theory]
    [InlineData("", "", "", "", "", "")]
    [InlineData("name1", "role1", "/api/ApiGroup/org/default/action/GetSomething", "method1", "ID1", "zzz")]
    [InlineData("test2", "", "/api/ApiGroup/org/default/action/GetSomething", "", "", "zzz")]
    [InlineData("test2", "role2", "", "", "", "zzz")]
    [InlineData("test2", "role2", "/api/ApiGroup/org/default/action/GetSomething", "", "", "zzz")]
    [InlineData("test2", "role2", "/api/ApiGroup/org/default/action/GetSomething", "method1", "", "zzz")]
    [InlineData("", "role2", "/api/ApiGroup/org/default/action/GetSomething", "method1", "aaaa", "zzz")]
    public void TaskCompleteIfNoClaimData(string name, string role, string uri, string method, string groupSid, string nameId)
    {
        var m = new Mock<IRoleService>();
        var h = new GenericRbacHandlerChild(m.Object);

        var req = new GenericRbacRequirement();
        var reqs = new GenericRbacRequirement[0];

        var claims = new List<Claim>();

        AppendClaim(claims, ClaimTypes.NameIdentifier, nameId);
        AppendClaim(claims, ClaimTypes.Name, name);
        AppendClaim(claims, ClaimTypes.Role, role);
        AppendClaim(claims, ClaimTypes.AuthenticationMethod, method);
        AppendClaim(claims, ClaimTypes.Uri, uri);
        AppendClaim(claims, ClaimTypes.GroupSid, groupSid);

        var identity = new ClaimsIdentity(claims, "");
        var principle = new ClaimsPrincipal(identity);
        var ctx = new AuthorizationHandlerContext(reqs, principle, null);

        var t = h.HandleRequirement(ctx, req);

        Assert.NotNull(t);
        Assert.Equal(Task.CompletedTask, t);
    }

    [Theory]
    [InlineData("user1", "role2", "/api/ApiGroup/org/default/action/AdminDoSomething", "method1", "default")]
    public void TaskCompleteIfAdminApiNotGlobal(string name, string role, string uri, string method, string groupSid)
    {
        var m = new Mock<IRoleService>();
        var h = new GenericRbacHandlerChild(m.Object);

        var req = new GenericRbacRequirement();
        var reqs = new GenericRbacRequirement[0];

        var claims = new List<Claim>();

        AppendClaim(claims, ClaimTypes.NameIdentifier, name);
        AppendClaim(claims, ClaimTypes.Name, name);
        AppendClaim(claims, ClaimTypes.Role, role);
        AppendClaim(claims, ClaimTypes.AuthenticationMethod, method);
        AppendClaim(claims, ClaimTypes.Uri, uri);
        AppendClaim(claims, ClaimTypes.GroupSid, groupSid);

        var identity = new ClaimsIdentity(claims, "");
        var principle = new ClaimsPrincipal(identity);
        var ctx = new AuthorizationHandlerContext(reqs, principle, null);

        var t = h.HandleRequirement(ctx, req);

        Assert.NotNull(t);
        Assert.Equal(Task.CompletedTask, t);
    }

    [Theory]
    [InlineData("user1", "OWNER", "/api/ApiGroup/org/default/action/AddSomething", "method1", "default")]
    [InlineData("user2", "VIEWER", "/api/ApiGroup/org/default/action/GetDoSomething", "method1", "default")]
    [InlineData("user3", "OWNER", "/api/ApiGroup/org/global/action/DeleteDoSomething", "method1", "global")]
    public void DoneWithRoleMatchApi(string name, string role, string uri, string method, string groupSid)
    {
        var m = new Mock<IRoleService>();
        m.Setup(x => x.GetRolesList("", "OWNER")).Returns(ownerRole);
        m.Setup(x => x.GetRolesList("", "VIEWER")).Returns(viewerRole);
        m.Setup(x => x.GetRolesList("", "NOTHING")).Returns(annonymousRole);

        var h = new GenericRbacHandlerChild(m.Object);

        var req = new GenericRbacRequirement();
        var reqs = new GenericRbacRequirement[0];

        var claims = new List<Claim>();

        AppendClaim(claims, ClaimTypes.NameIdentifier, name);
        AppendClaim(claims, ClaimTypes.Name, name);
        AppendClaim(claims, ClaimTypes.Role, role);
        AppendClaim(claims, ClaimTypes.AuthenticationMethod, method);
        AppendClaim(claims, ClaimTypes.Uri, uri);
        AppendClaim(claims, ClaimTypes.GroupSid, groupSid);

        var identity = new ClaimsIdentity(claims, "");
        var principle = new ClaimsPrincipal(identity);
        var ctx = new AuthorizationHandlerContext(reqs, principle, new DefaultHttpContext());

        var t = h.HandleRequirement(ctx, req);
        var mvcCtx = ctx.Resource as DefaultHttpContext;

        Assert.NotNull(t);
        Assert.NotNull(mvcCtx);
        Assert.Equal(role, mvcCtx.Items["Temp-Authorized-Role"]);
        Assert.Equal(method, mvcCtx.Items["Temp-Identity-Type"]);
        Assert.Contains("Temp-API-Called", mvcCtx.Items);
        Assert.Contains("Temp-Identity-Id", mvcCtx.Items);
    }

    [Theory]
    [InlineData("user1", "NOTHING", "/api/ApiGroup/org/default/action/GetSomething", "method1", "default")]
    [InlineData("user1", "NOTHING", "/api/ApiGroup/org/global/action/AetSomething", "method1", "global")]
    public void TaskCompleteIfRoleNotMatchApi(string name, string role, string uri, string method, string groupSid)
    {
        var m = new Mock<IRoleService>();
        m.Setup(x => x.GetRolesList("", "NOTHING")).Returns(annonymousRole);

        var h = new GenericRbacHandlerChild(m.Object);

        var req = new GenericRbacRequirement();
        var reqs = new GenericRbacRequirement[0];

        var claims = new List<Claim>();

        AppendClaim(claims, ClaimTypes.NameIdentifier, name);
        AppendClaim(claims, ClaimTypes.Name, name);
        AppendClaim(claims, ClaimTypes.Role, role);
        AppendClaim(claims, ClaimTypes.AuthenticationMethod, method);
        AppendClaim(claims, ClaimTypes.Uri, uri);
        AppendClaim(claims, ClaimTypes.GroupSid, groupSid);

        var identity = new ClaimsIdentity(claims, "");
        var principle = new ClaimsPrincipal(identity);
        var ctx = new AuthorizationHandlerContext(reqs, principle, new DefaultHttpContext());

        var t = h.HandleRequirement(ctx, req);

        Assert.NotNull(t);
        Assert.Equal(Task.CompletedTask, t);
    }
}
