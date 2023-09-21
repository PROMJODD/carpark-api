using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Prom.LPR.Api.Models;
using Prom.LPR.Api.Services;

namespace Prom.LPR.Api.Controllers
{
    [ApiController]
    [Authorize(Policy = "GenericRolePolicy")]
    [Route("/api/[controller]")]
    public class OrganizationController : ControllerBase
    {
        private readonly IConfiguration cfg;
        private readonly IOrganizationService svc;

        [ExcludeFromCodeCoverage]
        public OrganizationController(IOrganizationService service, IConfiguration configuration)
        {
            cfg = configuration;
            svc = service;
        }

        [ExcludeFromCodeCoverage]
        [HttpGet]
        [Route("org/{id}/action/GetOrganization")]
        public async Task<IActionResult> GetOrganization(string id)
        {
            var result = await svc.GetOrganization(id);
            return Ok(result);
        }

        [ExcludeFromCodeCoverage]
        [HttpPost]
        [Route("org/{id}/action/AddUserToOrganization")]
        public IActionResult AddUserToOrganization(string id, [FromBody] MOrganizationUser request)
        {
            var result = svc.AddUserToOrganization(id, request);
            return Ok(result);
        }
    }
}
