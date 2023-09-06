using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Prom.LPR.Api.Models;

namespace Prom.LPR.Api.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    //[Authorize]
    public class FileUploadController : ControllerBase
    {
        private readonly IConfiguration cfg;

        public FileUploadController(IConfiguration configuration)
        {
            cfg = configuration;
        }

        [HttpPost]
        [Route("org/{id}/action/UploadVehicleImage")]
        public IActionResult UploadVehicleImage(string id)
        {
            var r = new MVehicle() 
            {
                License = "กท-234-0999",
                Province = "กรุงเทพมหานคร"
            };

            return Ok(r);
        }
    }
}
