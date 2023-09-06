using Microsoft.AspNetCore.Mvc;
using Prom.LPR.Api.Models;
using Serilog;

namespace Prom.LPR.Api.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class FileUploadController : ControllerBase
    {
        private readonly IConfiguration cfg;

        public FileUploadController(IConfiguration configuration)
        {
            cfg = configuration;
        }

        [HttpPost]
        [Route("org/{id}/action/UploadVehicleImage")]
        public IActionResult UploadVehicleImage(string id, [FromForm] MImageUploaded img)
        {
            var image = img.Image;
            if (image == null)
            {
                Log.Information($"No uploaded file available");
                return NotFound();
            }

            Log.Information($"Uploaded file --> [{image.FileName}]");

            var r = new MVehicle() 
            {
                License = "กท-234-0999",
                Province = "กรุงเทพมหานคร"
            };

            return Ok(r);
        }
    }
}
