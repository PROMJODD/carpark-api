using System.Security.Claims;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using Prom.LPR.Api.Services;
using Serilog;

namespace Prom.LPR.Api.Authorizations;

public class GenericRbacHandler : AuthorizationHandler<GenericRbacRequirement>
{
    private readonly IRoleService service;
    private string apiCalled = "";
    private readonly string adminOnlyApiPattern = @"^(.+):(Admin.+)$";

    public GenericRbacHandler(IRoleService svc)
    {
        service = svc;
    }

    private static Claim? GetClaim(string type, IEnumerable<Claim> claims)
    {
        var claim = claims.FirstOrDefault(x => x.Type == type);
        return claim;
    }

    private string? IsRoleValid(IEnumerable<Models.MRole>? roles, string uri)
    {
        var uriPattern = @"^\/api\/(.+)\/org\/(.+)\/action\/(.+)$";
        var matches = Regex.Matches(uri, uriPattern, RegexOptions.None, TimeSpan.FromMilliseconds(100));

        var group = matches[0].Groups[1].Value;
        var api = matches[0].Groups[3].Value;

        var keyword = $"{group}:{api}";
        apiCalled = keyword;

        foreach (var role in roles!)
        {
            var patterns = role.RoleDefinition!.Split(',').ToList();
            foreach (var pattern in patterns!)
            {
                Match m = Regex.Match(keyword, pattern, RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(100));
                if (m.Success)
                {
                    return role.RoleName;
                }
            }
        }

        return "";
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, GenericRbacRequirement requirement)
    {
        var idClaim = GetClaim(ClaimTypes.NameIdentifier, context.User.Claims);
        if (idClaim == null)
        {
            //The authentication failed earlier
            return Task.CompletedTask;
        }

        var roleClaim = GetClaim(ClaimTypes.Role, context.User.Claims);
        if (roleClaim == null)
        {
            //The authentication failed earlier
            return Task.CompletedTask;
        }

        var uriClaim = GetClaim(ClaimTypes.Uri, context.User.Claims);
        if (uriClaim == null)
        {
            //The authentication failed earlier
            return Task.CompletedTask;
        }

        var authMethodClaim = GetClaim(ClaimTypes.AuthenticationMethod, context.User.Claims);
        if (authMethodClaim == null)
        {
            //The authentication failed earlier
            return Task.CompletedTask;
        }

        var orgIdClaim = GetClaim(ClaimTypes.GroupSid, context.User.Claims);
        if (orgIdClaim == null)
        {
            //The authentication failed earlier
            return Task.CompletedTask;
        }

        var uid = idClaim.Value;
        var role = roleClaim.Value;
        var uri = uriClaim.Value;
        var method = authMethodClaim.Value;
        var authorizeOrgId = orgIdClaim.Value;

        var roles = service.GetRolesList("", role);
        var roleMatch = IsRoleValid(roles, uri);

        Match m = Regex.Match(apiCalled, adminOnlyApiPattern, RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(100));
        if (m.Success && !authorizeOrgId.Equals("global"))
        {
            //Reject if API is match Admin(.+) but ID is not in "global" organization
            Log.Warning($"Invoked API [{apiCalled}] for UID [{uid}] [{method}] with org [{authorizeOrgId}] is not allowed!!!");
            return Task.CompletedTask;
        }

        if (!roleMatch!.Equals(""))
        {
            context.Succeed(requirement);

            var mvcContext = context.Resource as DefaultHttpContext;
            mvcContext!.HttpContext.Items["Temp-Authorized-Role"] = roleMatch;
            mvcContext!.HttpContext.Items["Temp-API-Called"] = apiCalled;
            mvcContext!.HttpContext.Items["Temp-Identity-Type"] = method;
            mvcContext!.HttpContext.Items["Temp-Identity-Id"] = uid;
        }

        return Task.CompletedTask;
    }
}
