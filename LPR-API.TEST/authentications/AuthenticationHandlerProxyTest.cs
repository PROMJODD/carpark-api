using Moq;
using Xunit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Encodings.Web;
using Microsoft.Extensions.Options;
using System.Text;
using Prom.LPR.Api.Authentications;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Prom.LPR.Test.Api.Authentications;

public class AuthenticationHandlerProxyTest
{
    private Mock<IOptionsMonitor<AuthenticationSchemeOptions>>? options;
    private Mock<ILoggerFactory>? loggerFactory;
    private Mock<UrlEncoder> encoder = new Mock<UrlEncoder>();
    private Mock<ISystemClock> clock = new Mock<ISystemClock>();
    private IConfiguration cfg = new Mock<IConfiguration>().Object;
    private Mock<IBasicAuthenticationRepo>? basicRepo = new Mock<IBasicAuthenticationRepo>();
    private Mock<IBearerAuthenticationRepo>? bearerRepo = new Mock<IBearerAuthenticationRepo>();

    private AuthenticationHandlerProxyChild GetHandler(HttpContext ctx)
    {
        options = new Mock<IOptionsMonitor<AuthenticationSchemeOptions>>();
        options.Setup(x => x.Get(It.IsAny<string>())).Returns(new AuthenticationSchemeOptions());

        var logger = new Mock<ILogger<AuthenticationHandlerProxyChild>>();
        loggerFactory = new Mock<ILoggerFactory>();
        loggerFactory.Setup(x => x.CreateLogger(It.IsAny<String>())).Returns(logger.Object);

        var handler = new AuthenticationHandlerProxyChild(options.Object, loggerFactory.Object, encoder.Object, 
            basicRepo!.Object, bearerRepo!.Object, cfg, clock.Object);

        handler.InitializeAsync(new AuthenticationScheme("", null, typeof(AuthenticationHandlerProxyChild)), ctx);

        return handler;
    }

    [Fact]
    public void FailIfNoAuthorizationHeaderFound()
    {
        var context = new DefaultHttpContext();
        var handler = GetHandler(context);

        Task<AuthenticateResult> task = handler.HandleAuthenticateAsyncWrap();

        Assert.NotNull(task);
        Assert.NotNull(task.Result.Failure);
        Assert.Equal("No Authorization header found", task.Result.Failure.Message);
    }

    [Fact]
    public void FailIfNoBasicOrBearerFound()
    {
        var context = new DefaultHttpContext();
        context.Request.Headers.Add("Authorization", "XXXX");

        var handler = GetHandler(context);
        Task<AuthenticateResult> task = handler.HandleAuthenticateAsyncWrap();

        Assert.NotNull(task);
        Assert.NotNull(task.Result.Failure);
        Assert.NotNull(task.Result.Failure.Message);
        Assert.Contains("Unknown scheme", task.Result.Failure.Message);
    }

    [Theory]
    [InlineData("user1", "password1", "default", "/api/ApiGroup/org/default/action/GetSomething")]
    [InlineData("00001", "user2", "00001", "/api/ApiGroup/org/00001/action/GetSomething")]
    public void BasicAuthenSuccess(string user, string password, string orgId, string uri)
    {
        string credential = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{user}:{password}"));
        string authStr = $"Basic {credential}";

        var context = new DefaultHttpContext();
        context.Request.Headers.Add("Authorization", authStr);
        context.Request.Path = uri;

        var authenUser = new User() { UserName = user, OrgId = orgId };
        basicRepo = new Mock<IBasicAuthenticationRepo>();
        basicRepo.Setup(x => x.Authenticate(orgId, user, password, It.IsAny<HttpRequest>())).Returns(authenUser);

        var handler = GetHandler(context);
        Task<AuthenticateResult> task = handler.HandleAuthenticateAsyncWrap();

        Assert.NotNull(task);
        Assert.True(task.Result.Succeeded);

        var ticket = task.Result.Ticket;
        Assert.NotNull(ticket);
        Assert.NotNull(ticket.Principal);

        //Future : Check the returned claims values
    }

    [Theory]
    [InlineData("user1", "password1", "default", "/api/ApiGroup/org/default/action/GetSomething")]
    [InlineData("00001", "user2", "00001", "/api/ApiGroup/org/00001/action/GetSomething")]
    public void BasicAuthenFail(string user, string password, string orgId, string uri)
    {
        string credential = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{user}:{password}"));
        string authStr = $"Basic {credential}";

        var context = new DefaultHttpContext();
        context.Request.Headers.Add("Authorization", authStr);
        context.Request.Path = uri;

        User? authenUser = null;
        basicRepo = new Mock<IBasicAuthenticationRepo>();
        basicRepo.Setup(x => x.Authenticate(orgId, user, password, It.IsAny<HttpRequest>())).Returns(authenUser);

        var handler = GetHandler(context);
        Task<AuthenticateResult> task = handler.HandleAuthenticateAsyncWrap();

        Assert.NotNull(task);
        Assert.NotNull(task.Result.Failure);
        Assert.NotNull(task.Result.Failure.Message);
        Assert.Contains("Invalid username or password", task.Result.Failure.Message);
    }

    [Theory]
    [InlineData("/api/ApiGroup/org/default/action/GetSomething")]
    [InlineData("/api/ApiGroup/org/00001/action/GetSomething")]
    public void BasicAuthenInvalidValue(string uri)
    {
        string authStr = $"Basic ThisIsNotValidString";

        var context = new DefaultHttpContext();
        context.Request.Headers.Add("Authorization", authStr);
        context.Request.Path = uri;

        var handler = GetHandler(context);
        Task<AuthenticateResult> task = handler.HandleAuthenticateAsyncWrap();

        Assert.NotNull(task);
        Assert.NotNull(task.Result.Failure);
        Assert.NotNull(task.Result.Failure.Message);
        Assert.Contains("Invalid Authorization Header", task.Result.Failure.Message);
    }

    //@@@@ Bearer @@@@

    [Theory]
    [InlineData("user1", "default", "/api/ApiGroup/org/default/action/GetSomething", "thisisjwt")]
    [InlineData("user2", "0000001", "/api/ApiGroup/org/00001/action/GetSomething", "dGhpc2lzand0Cg==")]
    public void BearerAuthenFailJwtNotValid(string user, string orgId, string uri, string jwt)
    {
        string authStr = $"Bearer {jwt}";

        var context = new DefaultHttpContext();
        context.Request.Headers.Add("Authorization", authStr);
        context.Request.Path = uri;

        User? authenUser = null;
        bearerRepo = new Mock<IBearerAuthenticationRepo>();
        bearerRepo.Setup(x => x.Authenticate(orgId, user, "", It.IsAny<HttpRequest>())).Returns(authenUser);

        var handler = GetHandler(context);
        Task<AuthenticateResult> task = handler.HandleAuthenticateAsyncWrap();

        Assert.NotNull(task);
        Assert.NotNull(task.Result.Failure);
        Assert.NotNull(task.Result.Failure.Message);
        Assert.Contains("Invalid Authorization Header", task.Result.Failure.Message);
    }

    private IConfiguration GetCofigFromDictionary(Dictionary<string, string> setting)
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(setting!)
            .Build();

        return configuration;
    }

    private string GetJwt(RsaSecurityKey key, string issuer, string audience, string user, int hourAddExpire)
    {
        var claims = new Dictionary<string, object>();
        claims.Add("preferred_username", user);

        var tokenHandler = new JsonWebTokenHandler();        
        var jwt = new SecurityTokenDescriptor()
        {
            Issuer = issuer,
            Audience = audience,
            SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.RsaSha256),
            IssuedAt = DateTime.Now,
            NotBefore = DateTime.Now,
            Expires = DateTime.Now.AddHours(hourAddExpire),
            Claims = claims,
        };

        string jws = tokenHandler.CreateToken(jwt);
        return jws;
    }

    [Theory]
    [InlineData("user1", "default", "/api/ApiGroup/org/default/action/GetSomething")]
    [InlineData("user2", "0000001", "/api/ApiGroup/org/0000001/action/GetSomething")]
    public void BearerAuthenSuccess(string user, string orgId, string uri)
    {
        var issuer = "https://keycloak.promid.prom.co.th/auth/realms/promid";
        var audience = "account";

        var setting = new Dictionary<string, string>
        {
            {"SSO:issuer", issuer},
            {"SSO:audience", audience}
        };

        var key = new RsaSecurityKey(RSA.Create(2048));

        var jwt = GetJwt(key, issuer, audience, user, 1);
        var jwtBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{jwt}"));
        string authStr = $"Bearer {jwtBase64}";

        var context = new DefaultHttpContext();
        context.Request.Headers.Add("Authorization", authStr);
        context.Request.Path = uri;

        var authenUser = new User() { UserName = user, OrgId = orgId };
        bearerRepo = new Mock<IBearerAuthenticationRepo>();
        bearerRepo.Setup(x => x.Authenticate(orgId, user, "", It.IsAny<HttpRequest>())).Returns(authenUser);

        SecurityKey securityKey = new RsaSecurityKey(key.Rsa.ExportParameters(false));
        var jwtSigner = new Mock<IJwtSigner>();
        jwtSigner.Setup(x => x.GetSignedKey(It.IsAny<string>())).Returns(securityKey);

        cfg = GetCofigFromDictionary(setting);

        var handler = GetHandler(context);
        handler.SetJwtSigner(jwtSigner.Object);
        Task<AuthenticateResult> task = handler.HandleAuthenticateAsyncWrap();

        cfg = new Mock<IConfiguration>().Object;

        Assert.NotNull(task);
        Assert.True(task.Result.Succeeded);

        var ticket = task.Result.Ticket;
        Assert.NotNull(ticket);
        Assert.NotNull(ticket.Principal);
    }

    [Theory]
    [InlineData("user1", "default", "/api/ApiGroup/org/default/action/GetSomething")]
    [InlineData("user2", "global", "/api/ApiGroup/org/global/action/GetSomething")]
    public void BearerAuthenFailJwtExpire(string user, string orgId, string uri)
    {
        var issuer = "https://keycloak.promid.prom.co.th/auth/realms/promid";
        var audience = "account";

        var setting = new Dictionary<string, string>
        {
            {"SSO:issuer", issuer},
            {"SSO:audience", audience}
        };

        var key = new RsaSecurityKey(RSA.Create(2048));

        var jwt = GetJwt(key, issuer, audience, user, -1);
        var jwtBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{jwt}"));
        string authStr = $"Bearer {jwtBase64}";

        var context = new DefaultHttpContext();
        context.Request.Headers.Add("Authorization", authStr);
        context.Request.Path = uri;

        var authenUser = new User() { UserName = user, OrgId = orgId };
        bearerRepo = new Mock<IBearerAuthenticationRepo>();
        bearerRepo.Setup(x => x.Authenticate(orgId, user, "", It.IsAny<HttpRequest>())).Returns(authenUser);

        SecurityKey securityKey = new RsaSecurityKey(key.Rsa.ExportParameters(false));
        var jwtSigner = new Mock<IJwtSigner>();
        jwtSigner.Setup(x => x.GetSignedKey(It.IsAny<string>())).Returns(securityKey);

        cfg = GetCofigFromDictionary(setting);

        var handler = GetHandler(context);
        handler.SetJwtSigner(jwtSigner.Object);
        Task<AuthenticateResult> task = handler.HandleAuthenticateAsyncWrap();

        cfg = new Mock<IConfiguration>().Object;

        Assert.NotNull(task);
        Assert.NotNull(task.Result.Failure);
        Assert.NotNull(task.Result.Failure.Message);
        Assert.Contains("Invalid Authorization Header", task.Result.Failure.Message);
    }

    [Theory]
    [InlineData("user1", "default", "/api/ApiGroup/org/default/action/GetSomething")]
    [InlineData("user2", "global", "/api/ApiGroup/org/global/action/GetSomething")]
    public void BearerAuthenFailJwtNotTrusted(string user, string orgId, string uri)
    {
        var issuer = "https://keycloak.promid.prom.co.th/auth/realms/promid";
        var audience = "account";

        var setting = new Dictionary<string, string>
        {
            {"SSO:issuer", issuer},
            {"SSO:audience", audience}
        };

        var key = new RsaSecurityKey(RSA.Create(2048));
        var keyUntrusted = new RsaSecurityKey(RSA.Create(2048));

        var jwt = GetJwt(key, issuer, audience, user, 1);
        var jwtBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{jwt}"));
        string authStr = $"Bearer {jwtBase64}";

        var context = new DefaultHttpContext();
        context.Request.Headers.Add("Authorization", authStr);
        context.Request.Path = uri;

        var authenUser = new User() { UserName = user, OrgId = orgId };
        bearerRepo = new Mock<IBearerAuthenticationRepo>();
        bearerRepo.Setup(x => x.Authenticate(orgId, user, "", It.IsAny<HttpRequest>())).Returns(authenUser);

        SecurityKey securityKey = new RsaSecurityKey(keyUntrusted.Rsa.ExportParameters(false));
        var jwtSigner = new Mock<IJwtSigner>();
        jwtSigner.Setup(x => x.GetSignedKey(It.IsAny<string>())).Returns(securityKey);

        cfg = GetCofigFromDictionary(setting);

        var handler = GetHandler(context);
        handler.SetJwtSigner(jwtSigner.Object);
        Task<AuthenticateResult> task = handler.HandleAuthenticateAsyncWrap();

        cfg = new Mock<IConfiguration>().Object;

        Assert.NotNull(task);
        Assert.NotNull(task.Result.Failure);
        Assert.NotNull(task.Result.Failure.Message);
        Assert.Contains("Invalid Authorization Header", task.Result.Failure.Message);
    }


    [Theory]
    [InlineData("user1", "default", "/api/ApiGroup/org/default/action/GetSomething")]
    [InlineData("user2", "global", "/api/ApiGroup/org/global/action/GetSomething")]
    public void BearerAuthenFailWrongIssuerAudience(string user, string orgId, string uri)
    {
        var issuer = "https://keycloak.promid.prom.co.th/auth/realms/promid";
        var audience = "account";

        var setting = new Dictionary<string, string>
        {
            {"SSO:issuer", $"{issuer}.fake"},
            {"SSO:audience", $"{audience}.fake"}
        };

        var key = new RsaSecurityKey(RSA.Create(2048));
        var keyUntrusted = new RsaSecurityKey(RSA.Create(2048));

        var jwt = GetJwt(key, issuer, audience, user, 1);
        var jwtBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{jwt}"));
        string authStr = $"Bearer {jwtBase64}";

        var context = new DefaultHttpContext();
        context.Request.Headers.Add("Authorization", authStr);
        context.Request.Path = uri;

        var authenUser = new User() { UserName = user, OrgId = orgId };
        bearerRepo = new Mock<IBearerAuthenticationRepo>();
        bearerRepo.Setup(x => x.Authenticate(orgId, user, "", It.IsAny<HttpRequest>())).Returns(authenUser);

        SecurityKey securityKey = new RsaSecurityKey(keyUntrusted.Rsa.ExportParameters(false));
        var jwtSigner = new Mock<IJwtSigner>();
        jwtSigner.Setup(x => x.GetSignedKey(It.IsAny<string>())).Returns(securityKey);

        cfg = GetCofigFromDictionary(setting);

        var handler = GetHandler(context);
        handler.SetJwtSigner(jwtSigner.Object);
        Task<AuthenticateResult> task = handler.HandleAuthenticateAsyncWrap();

        cfg = new Mock<IConfiguration>().Object;

        Assert.NotNull(task);
        Assert.NotNull(task.Result.Failure);
        Assert.NotNull(task.Result.Failure.Message);
        Assert.Contains("Invalid Authorization Header", task.Result.Failure.Message);
    }
}
