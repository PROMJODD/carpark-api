using Microsoft.AspNetCore.Authorization;
using Prom.LPR.Api.Authorizations;
using Prom.LPR.Api.Services;

namespace Prom.LPR.Test.Api.Authorizations;

public class GenericRbacHandlerChild : GenericRbacHandler
{
    public GenericRbacHandlerChild(IRoleService svc) : base(svc)
    {
    }

    public Task HandleRequirement(AuthorizationHandlerContext context, GenericRbacRequirement requirement)
    {
        var t = HandleRequirementAsync(context, requirement);
        return t;
    }
}
