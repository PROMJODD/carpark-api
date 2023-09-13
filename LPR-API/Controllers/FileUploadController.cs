using Serilog;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using Prom.LPR.Api.Models;
using Prom.LPR.Api.Utils;
using Prom.LPR.Api.Kafka;
using System.Text.Json;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Microsoft.AspNetCore.Authorization;
using Prom.LPR.Api.Services;
using Prom.LPR.Api.ViewsModels;

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
        private Producer<MKafkaMessage> producer;

        public FileUploadController(IConfiguration configuration, 
            IFileUploadedService svc)
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

            producer = new Producer<MKafkaMessage>(kafkaHost, kafkaPort);
        }

        private HttpClient GetHttpClient()
        {
            var client = new HttpClient();
            Uri baseUri = new Uri(lprBaseUrl);
            client.BaseAddress = baseUri;
            client.Timeout = TimeSpan.FromMilliseconds(1000);

            return client;
        }

        private HttpRequestMessage GetRequestMessage()
        {
            //Bearer Authentication
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, lprPath);
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", lprAuthKey);
            var productValue = new ProductInfoHeaderValue("lpr-api", "1.0");
            requestMessage.Headers.UserAgent.Add(productValue);

            return requestMessage;
        }

        private void PublishMessage(MKafkaMessage data)
        {
            producer.Produce(data, topic);
        }

        private MStorageData UploadFile(string localPath, string org, string folder) 
        {
            var objectName = Path.GetFileName(localPath);
            string objectPath = $"{org}/{folder}/{objectName}";
            string gcsPath = $"gs://{imagesBucket}/{objectPath}";

            Log.Information($"Uploading file [{localPath}] to [{gcsPath}]");

            StorageClient storageClient = StorageClient.Create();
            using (var f = System.IO.File.OpenRead(localPath))
            {
                storageClient.UploadObject(imagesBucket, $"{objectPath}", null, f);
            }

            var url = "";
            try
            {
                var credential = GoogleCredential.GetApplicationDefault();
                var urlSigner = UrlSigner.FromCredential(credential);
                url = urlSigner.Sign(imagesBucket, objectPath, TimeSpan.FromHours(1), HttpMethod.Get);
            }
            catch (Exception e)
            {
                Log.Error($"Unable to sign URL - [{gcsPath}]");
                Log.Error(e.Message);
            }

            var storageObj = new MStorageData() 
            {
                StoragePath = gcsPath,
                PreSignedUrl = url
            };

            return storageObj;
        }

        private string LPRAnalyzeFile(string imagePath)
        {
            var client = GetHttpClient();
            var requestMessage = GetRequestMessage();

            using var stream = System.IO.File.OpenRead(imagePath);
            using var content = new MultipartFormDataContent
            {
                { new StreamContent(stream), "image", imagePath }
            };

            requestMessage.Content = content;
            var task = client.SendAsync(requestMessage);
            var response = task.Result;

            var lprResult = "";
            try
            {
                response.EnsureSuccessStatusCode();
                string responseBody = response.Content.ReadAsStringAsync().Result;

                lprResult = responseBody;
                Console.WriteLine($"{responseBody}");
            }
            catch (Exception e)
            {
                string responseBody = response.Content.ReadAsStringAsync().Result;
                Log.Error(responseBody);
                Log.Error(e.Message);
            }

            return lprResult;
        }

        private MLPRResult? GetLPRObject(string json)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var obj = JsonSerializer.Deserialize<MLPRResult>(json, options);
            return obj;
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
            var msg = LPRAnalyzeFile(tmpFile);
            var lprObj = GetLPRObject(msg);


            var dateStamp = DateTime.Now.ToString("yyyyMMddhh");
            var folder = $"{dateStamp}";
            var storageObj = UploadFile(tmpFile, id, folder);

            var data = new MKafkaMessage() 
            {
                LprData = lprObj,
                StorageData = storageObj,
                HttpRequestHeader = Request.Headers
            };

            AddRecord(id, data, tmpFile);
            PublishMessage(data);

            var resp = new MLPRResponse() 
            {
                LprData = lprObj,
                StorageData = storageObj,
            };

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
    }
}
