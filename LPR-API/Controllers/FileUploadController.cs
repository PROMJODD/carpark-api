using Serilog;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using Prom.LPR.Api.Models;
using Prom.LPR.Api.Utils;

namespace Prom.LPR.Api.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class FileUploadController : ControllerBase
    {
        private readonly IConfiguration cfg;
        private string lprBaseUrl = "";
        private string lprPath = "";
        private string lprAuthKey = "";
        private string imagesBucket = "";

        public FileUploadController(IConfiguration configuration)
        {
            cfg = configuration;
            imagesBucket = ConfigUtils.GetConfig(cfg, "LPR:bucket");
            lprBaseUrl = ConfigUtils.GetConfig(cfg, "LPR:lprBaseUrl");
            lprPath = ConfigUtils.GetConfig(cfg, "LPR:lprPath");
            lprAuthKey = ConfigUtils.GetConfig(cfg, "LPR:lprAuthKey");

            Log.Information($"LPR URL=[{lprBaseUrl}], LPR Path=[{lprPath}]");
        }

        private HttpClient GetHttpClient()
        {
            var client = new HttpClient();
            Uri baseUri = new Uri(lprBaseUrl);
            client.BaseAddress = baseUri;
            client.Timeout = TimeSpan.FromMinutes(0.05);

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
            LPRAnalyzeFile(tmpFile);

            var r = new MVehicle() 
            {
                License = "กท-234-0999",
                Province = "กรุงเทพมหานคร"
            };

            return Ok(r);
        }
    }
}
