using Moq;
using Xunit;
using Prom.LPR.Api.Services;
using Prom.LPR.Api.Authentications;
using Microsoft.AspNetCore.Http;
using Prom.LPR.Api.ModelsViews;
using Prom.LPR.Api.Models;
using System.Security.Claims;

namespace Prom.LPR.Test.Api.Authentications;

public class BearerAuthenticationRepoTest
{
    private Claim? GetClaim(string type, IEnumerable<Claim> claims)
    {
        var claim = claims.FirstOrDefault(x => x.Type == type);
        return claim;
    }

    [Theory]
    [InlineData("default", "user1", "/api/ApiGroup/org/default/action/GetSomething")]
    [InlineData("global", "user1", "/api/ApiGroup/org/global/action/GetSomething")]
    public void AuthenVerifyUserIsNull(string orgId, string user, string path)
    {
        MVOrganizationUser orgUser = null!;

        var m = new Mock<IOrganizationService>();
        m.Setup(x => x.VerifyUserInOrganization(orgId, user)).Returns(orgUser);

        var req = new Mock<HttpRequest>();
        req.Setup(x => x.Path).Returns(path);

        var repo = new BearerAuthenticationRepo(m.Object);
        var u = repo.Authenticate(orgId, user, "", req.Object);

        Assert.Null(u);
    }

    [Theory]
    [InlineData("default", "user1", "/api/ApiGroup/org/default/action/GetSomething")]
    [InlineData("00001", "user2", "/api/ApiGroup/org/00001/action/GetSomething")]
    public void AuthenVerifyUserNotOK(string orgId, string user, string path)
    {
        var orgUser = new MVOrganizationUser() { Status = "ANYTHING_NOT_OK", Description = "This is not OK" };

        var m = new Mock<IOrganizationService>();
        m.Setup(x => x.VerifyUserInOrganization(orgId, user)).Returns(orgUser);

        var req = new Mock<HttpRequest>();
        req.Setup(x => x.Path).Returns(path);

        var repo = new BearerAuthenticationRepo(m.Object);
        var u = repo.Authenticate(orgId, user, "", req.Object);

        Assert.Null(u);
    }

    [Theory]
    [InlineData("default", "user1", "/api/ApiGroup/org/default/action/GetSomething")]
    [InlineData("00001", "user2", "/api/ApiGroup/org/00001/action/GetSomething")]
    [InlineData("global", "user3", "/api/ApiGroup/org/global/action/GetSomething")]
    public void AuthenVerifyUserOK(string orgId, string user, string path)
    {
        var uid = Guid.NewGuid();
        var role = "A,B,C,D";
    
        var usr = new MUser() { UserId = uid };
        var org = new MOrganizationUser() { RolesList = role, OrgCustomId = orgId };
        var orgUser = new MVOrganizationUser() 
        { 
            Status = "OK", 
            Description = "Success",
            User = usr,
            OrgUser = org, 
        };

        var m = new Mock<IOrganizationService>();
        m.Setup(x => x.VerifyUserInOrganization(orgId, user)).Returns(orgUser);

        var req = new Mock<HttpRequest>();
        req.Setup(x => x.Path).Returns(path);

        var repo = new BearerAuthenticationRepo(m.Object);
        var u = repo.Authenticate(orgId, user, "", req.Object);

        Assert.NotNull(u);
        Assert.NotNull(u.Claims);
        Assert.Equal("JWT", u.AuthenType);
        Assert.Equal(role, u.Role);
        Assert.Equal(orgId, u.OrgId);
        Assert.Equal(user, u.UserName);

        var idClaim = GetClaim(ClaimTypes.NameIdentifier, u.Claims);
        Assert.Equal(u.UserId.ToString(), idClaim!.Value);

        var roleClaim = GetClaim(ClaimTypes.Role, u.Claims);
        Assert.Equal(role, roleClaim!.Value);

        var uriClaim = GetClaim(ClaimTypes.Uri, u.Claims);
        Assert.Equal(path, uriClaim!.Value);
    }
}
