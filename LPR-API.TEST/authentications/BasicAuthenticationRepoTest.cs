using Moq;
using Xunit;
using Prom.LPR.Api.Services;
using Prom.LPR.Api.Authentications;
using Microsoft.AspNetCore.Http;
using Prom.LPR.Api.ModelsViews;
using Prom.LPR.Api.Models;
using System.Security.Claims;

namespace Prom.LPR.Test.Api.Authentications;

public class BasicAuthenticationRepoTest
{
    private Claim? GetClaim(string type, IEnumerable<Claim> claims)
    {
        var claim = claims.FirstOrDefault(x => x.Type == type);
        return claim;
    }

    [Theory]
    [InlineData("default", "user1", "password1", "/api/ApiGroup/org/default/action/GetSomething")]
    public void AuthenVerifyApiKeyIsNull(string orgId, string user, string password, string path)
    {
        MVApiKey apiKey1 = null!;

        var m = new Mock<IApiKeyService>();
        m.Setup(x => x.VerifyApiKey("default", password)).Returns(apiKey1);
        m.Setup(x => x.VerifyApiKey("global", password)).Returns(apiKey1);

        var req = new Mock<HttpRequest>();
        req.Setup(x => x.Path).Returns(path);

        var repo = new BasicAuthenticationRepo(m.Object);
        var u = repo.Authenticate(orgId, user, password, req.Object);
        Assert.Null(u);
    }

    [Theory]
    [InlineData("default", "user1", "password1", "/api/ApiGroup/org/default/action/GetSomething")]
    [InlineData("00001", "user2", "password2", "/api/ApiGroup/org/00001/action/GetSomething")]
    public void AuthenVerifyApiKeyInOrgOK(string orgId, string user, string password, string path)
    {
        var id = Guid.NewGuid();
        var role = "A,B,C,D";
        var key = new MApiKey() { ApiKey = password, KeyId = id, RolesList = role, OrgId = orgId };
    
        var apiKey1 = new MVApiKey() { Status = "OK", ApiKey = key };

        var m = new Mock<IApiKeyService>();
        m.Setup(x => x.VerifyApiKey(orgId, password)).Returns(apiKey1);

        var req = new Mock<HttpRequest>();
        req.Setup(x => x.Path).Returns(path);

        var repo = new BasicAuthenticationRepo(m.Object);
        var u = repo.Authenticate(orgId, user, password, req.Object);

        Assert.NotNull(u);
        Assert.NotNull(u.claims);
        Assert.Equal("API-KEY", u.AuthenType);
        Assert.Equal(role, u.Role);
        Assert.Equal(orgId, u.OrgId);
        Assert.Equal(user, u.UserName);

        var idClaim = GetClaim(ClaimTypes.NameIdentifier, u.claims);
        Assert.Equal(u.UserId.ToString(), idClaim!.Value);

        var roleClaim = GetClaim(ClaimTypes.Role, u.claims);
        Assert.Equal(role, roleClaim!.Value);

        var uriClaim = GetClaim(ClaimTypes.Uri, u.claims);
        Assert.Equal(path, uriClaim!.Value);
    }


    [Theory]
    [InlineData("global", "user1", "password1", "/api/ApiGroup/org/global/action/GetSomething")]
    [InlineData("global", "user2", "password2", "/api/ApiGroup/org/global/action/GetSomething")]
    public void AuthenVerifyApiKeyInGlobalOrgOK(string orgId, string user, string password, string path)
    {
        var id = Guid.NewGuid();
        var role = "A,B,C,D";
        var key = new MApiKey() { ApiKey = password, KeyId = id, RolesList = role, OrgId = orgId };
    
        var apiKey2 = new MVApiKey() { Status = "OK", ApiKey = key };

        var m = new Mock<IApiKeyService>();
        m.Setup(x => x.VerifyApiKey("global", password)).Returns(apiKey2);

        var req = new Mock<HttpRequest>();
        req.Setup(x => x.Path).Returns(path);

        var repo = new BasicAuthenticationRepo(m.Object);
        var u = repo.Authenticate(orgId, user, password, req.Object);

        Assert.NotNull(u);
        Assert.NotNull(u.claims);
        Assert.Equal("API-KEY", u.AuthenType);
        Assert.Equal(role, u.Role);
        Assert.Equal(orgId, u.OrgId);
        Assert.Equal(user, u.UserName);

        var idClaim = GetClaim(ClaimTypes.NameIdentifier, u.claims);
        Assert.Equal(u.UserId.ToString(), idClaim!.Value);

        var roleClaim = GetClaim(ClaimTypes.Role, u.claims);
        Assert.Equal(role, roleClaim!.Value);

        var uriClaim = GetClaim(ClaimTypes.Uri, u.claims);
        Assert.Equal(path, uriClaim!.Value);
    }
}
