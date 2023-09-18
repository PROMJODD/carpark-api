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

namespace Prom.LPR.Test.Api.Authentications;

public class AuthenticationHandlerProxyTest
{
    private Mock<IOptionsMonitor<AuthenticationSchemeOptions>>? options;
    private Mock<ILoggerFactory>? loggerFactory;
    private Mock<UrlEncoder> encoder = new Mock<UrlEncoder>();
    private Mock<ISystemClock> clock = new Mock<ISystemClock>();
    private Mock<IConfiguration> cfg = new Mock<IConfiguration>();
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
            basicRepo!.Object, bearerRepo!.Object, cfg.Object, clock.Object);

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

        var authenUser = new User() { UserName = user, OrgId = orgId } ;
        basicRepo = new Mock<IBasicAuthenticationRepo>();
        basicRepo.Setup(x => x.Authenticate(orgId, user, password, It.IsAny<HttpRequest>())).Returns(authenUser);

        var handler = GetHandler(context);
        Task<AuthenticateResult> task = handler.HandleAuthenticateAsyncWrap();

        Assert.NotNull(task);
        Assert.True(task.Result.Succeeded);

        var ticket = task.Result.Ticket;
        Assert.NotNull(ticket);
        Assert.NotNull(ticket.Principal);

        //TODO : Check the returned claims values
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
}
