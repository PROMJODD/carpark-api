using Serilog;
using Microsoft.AspNetCore.Mvc;
using Prom.LPR.Api.Models;
using Prom.LPR.Api.Utils;
using Microsoft.AspNetCore.Authorization;
using Prom.LPR.Api.Services;
using Prom.LPR.Api.ViewsModels;
using System.Diagnostics.CodeAnalysis;
using Prom.LPR.Api.ExternalServices.Recognition;
using Prom.LPR.Api.ExternalServices.ObjectStorage;

namespace Prom.LPR.Api.Controllers
{
    [ExcludeFromCodeCoverage]
    [Authorize(Policy = "GenericRolePolicy")]
    [ApiController]
    [Route("/api/[controller]")]
    public class FileUploadController : ControllerBase
    {
        private readonly IConfiguration cfg;
        private readonly IFileUploadedService service;
        private string lprBaseUrl = "";
        private string lprPath = "";
        private string topic = "";
        private string kafkaHost = "";
        private string kafkaPort = "";

        private IImageAnalyzer analyzer;
        private GoogleCloudStorage gcs;

        public FileUploadController(IConfiguration configuration, IFileUploadedService svc)
        {
            service = svc;
            cfg = configuration;

            lprBaseUrl = ConfigUtils.GetConfig(cfg, "LPR:lprBaseUrl");
            lprPath = ConfigUtils.GetConfig(cfg, "LPR:lprPath");

            topic = ConfigUtils.GetConfig(cfg, "Kafka:topic");
            kafkaHost = ConfigUtils.GetConfig(cfg, "Kafka:host");
            kafkaPort = ConfigUtils.GetConfig(cfg, "Kafka:port");

            Log.Information($"LPR URL=[{lprBaseUrl}], LPR Path=[{lprPath}]");
            Log.Information($"Topic=[{topic}], Kafka Host=[{kafkaHost}], Kafka Port=[{kafkaPort}]");

            analyzer = new LPRAnalyzer(cfg);
            gcs = new GoogleCloudStorage();
            gcs.SetUrlSigner(GetSigner());
        }

        private GcsSigner? GetSigner()
        {
            try
            {
                return new GcsSigner();
            }
            catch
            {
                return null;
            }
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

            var result = service.GetFilesUploaded(id, param);
            return Ok(result);
        }

        [HttpGet]
        [Route("org/{id}/action/GetVehicleImagesCount")]
        public IActionResult GetVehicleImagesCount(string id, [FromQuery] VMFileUploadedQuery param)
        {
            param.UploadedApi = "FileUpload:UploadVehicleImage";

            var result = service.GetFilesUploadedCount(id, param);
            return Ok(result);
        }
    }
}
