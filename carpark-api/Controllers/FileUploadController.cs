using Serilog;
using Microsoft.AspNetCore.Mvc;
using Prom.LPR.Api.Models;
using Prom.LPR.Api.Utils;
using Microsoft.AspNetCore.Authorization;
using Prom.LPR.Api.Services;
using Prom.LPR.Api.ViewsModels;
using System.Diagnostics.CodeAnalysis;

namespace Prom.LPR.Api.Controllers
{
    [ExcludeFromCodeCoverage]
    [Authorize(Policy = "GenericRolePolicy")]
    [ApiController]
    [Route("/api/[controller]")]
    public class FileUploadController : ControllerBase
    {
        private readonly IFileUploadedService service;

        public FileUploadController(IConfiguration configuration, IFileUploadedService svc)
        {
            service = svc;
            var cfg = configuration;

            var lprBaseUrl = ConfigUtils.GetConfig(cfg, "LPR:lprBaseUrl");
            var lprPath = ConfigUtils.GetConfig(cfg, "LPR:lprPath");

            Log.Information($"LPR URL=[{lprBaseUrl}], LPR Path=[{lprPath}]");
        }

        [HttpPost]
        [Route("org/{id}/action/UploadVehicleImage")]
        public IActionResult UploadVehicleImage(string id, [FromForm] MImageUploaded img)
        {
            var resp = service.UploadFile(id, img, HttpContext);
            return Ok(resp);
        }

        [HttpGet]
        [Route("org/{id}/action/GetVehicleImages")]
        public IActionResult GetVehicleImages(string id, [FromQuery] VMFileUploadedQuery param)
        {
            param.UploadedApi = "FileUpload:UploadVehicleImage";
            if (param.Limit <= 0)
            {
                param.Limit = 100;
            }

            var result = service.GetFilesUploaded(id, param);
            return Ok(result);
        }

        [HttpGet]
        [Route("org/{id}/action/GetVehicleImagesCount")]
        public IActionResult GetVehicleImagesCount(string id, [FromQuery] VMFileUploadedQuery param)
        {
            param.UploadedApi = "FileUpload:UploadVehicleImage";
            if (param.Limit <= 0)
            {
                param.Limit = 100;
            }

            var result = service.GetFilesUploadedCount(id, param);
            return Ok(result);
        }
    }
}
