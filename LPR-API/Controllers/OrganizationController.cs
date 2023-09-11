using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Prom.LPR.Api.Services;

namespace Prom.LPR.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("/api/[controller]")]
    public class OrganizationController : ControllerBase
    {
        private readonly IConfiguration cfg;
        private readonly IOrganizationService svc;

        public OrganizationController(IOrganizationService service, IConfiguration configuration)
        {
            cfg = configuration;
            svc = service;
        }

        [HttpGet]
        [Route("org/{id}/action/GetOrganization")]
        public async Task<IActionResult> GetOrganization(string id)
        {
            var result = await svc.GetOrganization(id);
            return Ok(result);
        }
    }
}
