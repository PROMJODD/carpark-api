using Microsoft.AspNetCore.Mvc;
using Prom.LPR.Api.Services;

namespace Prom.LPR.Api.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class IamController : ControllerBase
    {
        private readonly IConfiguration cfg;
        private readonly IIamService svc;

        public IamController(IIamService service, IConfiguration configuration)
        {
            cfg = configuration;
            svc = service;
        }

        [HttpGet]
        //Check if API Key found & not expire in the specific organization
        [Route("org/{id}/action/VerifyApiKey/{apiKey}")]
        public IActionResult VerifyApiKey(string id, string apiKey)
        {
            var result = svc.VerifyApiKey(id, apiKey);
            return Ok(result);
        }
    }
}
