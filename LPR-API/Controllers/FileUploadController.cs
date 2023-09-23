using Serilog;
using Microsoft.AspNetCore.Mvc;
using Prom.LPR.Api.Models;
using Prom.LPR.Api.Utils;
using Microsoft.AspNetCore.Authorization;
using Prom.LPR.Api.Services;
using Prom.LPR.Api.ViewsModels;
using System.Diagnostics.CodeAnalysis;
using Prom.LPR.Api.ExternalServices.MessageQue;
using Prom.LPR.Api.ExternalServices.Recognition;
using Prom.LPR.Api.ExternalServices.ObjectStorage;

namespace Prom.LPR.Api.Controllers
{
    [Authorize(Policy = "GenericRolePolicy")]
    [ApiController]
    [Route("/api/[controller]")]
    public class FileUploadController : ControllerBase
    {
        private readonly IConfiguration cfg;
        private readonly IFileUploadedService service;
        private string lprBaseUrl = "";
        private string lprPath = "";
        private string lprAuthKey = "";
        private string imagesBucket = "";
        private string topic = "";
        private string kafkaHost = "";
        private string kafkaPort = "";

        private IImageAnalyzer analyzer;
        private GoogleCloudStorage gcs;

        public FileUploadController(IConfiguration configuration, IFileUploadedService svc)
        {
            service = svc;
            cfg = configuration;

            imagesBucket = ConfigUtils.GetConfig(cfg, "LPR:bucket");
            lprBaseUrl = ConfigUtils.GetConfig(cfg, "LPR:lprBaseUrl");
            lprPath = ConfigUtils.GetConfig(cfg, "LPR:lprPath");
            lprAuthKey = ConfigUtils.GetConfig(cfg, "LPR:lprAuthKey");

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

        private string GetContextValue(string key)
        {
            bool t = HttpContext.Items.TryGetValue(key, out object? e);
            if (t)
            {
                var value = e as string;
                return value!;
            }

            return "";
        }

        private void AddRecord(string id, MKafkaMessage data, string fname)
        {
            FileInfo fi = new FileInfo(fname);
            var m = new MFileUploaded()
            {
                OrgId = id,
                IdentityType = GetContextValue("Temp-Identity-Type"),
                UploaderId = GetContextValue("Temp-Identity-Id"),
                UploadedApi = GetContextValue("Temp-API-Called"),
                StoragePath = data!.StorageData!.StoragePath,
                RecognitionStatus = data!.LprData!.Status.ToString(),
                RecognitionMessage = data!.LprData.Message,
                VehicleLicense = data!.LprData!.Data!.License,
                VehicleProvince = data!.LprData!.Data!.Province,
                VehicleBrand = data!.LprData!.Data!.VehBrand,
                VehicleClass = data!.LprData!.Data!.VehClass,
                VehicleColor = data!.LprData!.Data!.VehColor,
                QuotaLeft = data!.LprData!.Data!.Remaining,
                FileSize = fi.Length,
            };

            service.AddFileUploaded(id, m);
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

            var ts = DateTime.Now.ToString("yyyyMMddhhmmss");
            var tmpFile = $"/tmp/{ts}.{image.FileName}";
            using (var fileStream = new FileStream(tmpFile, FileMode.Create))
            {
                image.CopyTo(fileStream);
            }

            Log.Information($"Uploaded file [{image.FileName}], saved to [{tmpFile}]");
            var lprObj = analyzer.AnalyzeFile<MLPRResult>(tmpFile);

            var dateStamp = DateTime.Now.ToString("yyyyMMddhh");
            var storageObj = gcs.UploadFile(tmpFile, id, imagesBucket, dateStamp);

            var data = new MKafkaMessage() 
            {
                LprData = lprObj,
                StorageData = storageObj,
                HttpRequestHeader = Request.Headers
            };

            AddRecord(id, data, tmpFile);

            var resp = new MLPRResponse() 
            {
                LprData = lprObj,
                StorageData = storageObj,
            };

            return Ok(resp);
        }

        [ExcludeFromCodeCoverage]
        [HttpGet]
        [Route("org/{id}/action/GetVehicleImages")]
        public IActionResult GetVehicleImages(string id, [FromQuery] VMFileUploadedQuery param)
        {
            param.UploadedApi = "FileUpload:UploadVehicleImage";

            var result = service.GetFilesUploaded(id, param);
            return Ok(result);
        }

        [ExcludeFromCodeCoverage]
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
