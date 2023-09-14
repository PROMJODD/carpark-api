using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Prom.LPR.Api.Models;
using Prom.LPR.Api.Services;

namespace Prom.LPR.Api.Controllers
{
    [ApiController]
    [Authorize(Policy = "GenericRolePolicy")]
    [Route("/api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IConfiguration cfg;
        private readonly IUserService svc;

        public UserController(IUserService service, IConfiguration configuration)
        {
            cfg = configuration;
            svc = service;
        }

        [HttpGet]
        [Route("org/{id}/action/AdminGetUsers")]
        public IActionResult AdminGetUsers(string id)
        {
            var result = svc.GetUsers(id);
            return Ok(result);
        }

        [HttpPost]
        [Route("org/{id}/action/AdminAddUser")]
        public IActionResult AdminAddUser(string id, [FromBody] MUser request)
        {
            var result = svc.AddUser(id, request);
            return Ok(result);
        }
    }
}
