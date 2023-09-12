using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Prom.LPR.Api.Models;
using Prom.LPR.Api.Services;

namespace Prom.LPR.Api.Controllers
{
    [Authorize(Policy = "GenericRolePolicy")]
    [ApiController]
    [Route("/api/[controller]")]
    public class ApiKeyController : ControllerBase
    {
        private readonly IConfiguration cfg;
        private readonly IApiKeyService svc;

        public ApiKeyController(IApiKeyService service, IConfiguration configuration)
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

        [HttpPost]
        [Route("org/{id}/action/AddApiKey")]
        public IActionResult AddApiKey(string id, [FromBody] MApiKey request)
        {
            var result = svc.AddApiKey(id, request);
            return Ok(result);
        }

        [HttpDelete]
        [Route("org/{id}/action/DeleteApiKeyById/{keyId}")]
        public IActionResult DeleteApiKeyById(string id, string keyId)
        {
            var result = svc.DeleteApiKeyById(id, keyId);
            return Ok(result);
        }

        // Use POST method, in the future we might send the body
        [HttpPost]
        [Route("org/{id}/action/GetApiKeys")]
        public IActionResult GetApiKeys(string id)
        {
            var result = svc.GetApiKeys(id);
            return Ok(result);
        }
    }
}
