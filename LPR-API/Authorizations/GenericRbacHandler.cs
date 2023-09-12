using System.Security.Claims;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Prom.LPR.Api.Services;

namespace Prom.LPR.Api.Authorizations;

public class GenericRbacHandler : AuthorizationHandler<GenericRbacRequirement>
{
    private readonly IRoleService service;

    public GenericRbacHandler(IRoleService svc)
    {
        service = svc;
    }

    private Claim? GetClaim(string type, IEnumerable<Claim> claims)
    {
        var claim = claims.FirstOrDefault(x => x.Type == type);
        return claim;
    }

    private string? IsRoleValid(IEnumerable<Models.MRole>? roles, string uri)
    {
        var uriPattern = @"^\/api\/(.+)\/org\/(.+)\/action\/(.+)$";
        var matches = Regex.Matches(uri, uriPattern);

        var group = matches[0].Groups[1].Value;
        var api = matches[0].Groups[3].Value;

        var keyword = $"{group}:{api}";

        foreach (var role in roles!)
        {
            var patterns = role.RoleDefinition!.Split(',').ToList();
            foreach (var pattern in patterns!)
            {
                Match m = Regex.Match(keyword, pattern, RegexOptions.IgnoreCase);
                if (m.Success)
                {
                    //Console.WriteLine($"### [{role.RoleName}] [{pattern}] [{keyword}] ###");
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

        //var uid = idClaim.Value;
        var role = roleClaim.Value;
        var uri = uriClaim.Value;

        var roles = service.GetRolesList("", role);

        var roleMatch = IsRoleValid(roles, uri);
        if (!roleMatch!.Equals(""))
        {
            context.Succeed(requirement);

            var mvcContext = context.Resource as DefaultHttpContext;
            mvcContext!.HttpContext.Items["Temp-Authorized-Role"] = roleMatch;
        }

        return Task.CompletedTask;
    }
}
