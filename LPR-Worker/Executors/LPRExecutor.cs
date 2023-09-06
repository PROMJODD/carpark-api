using Serilog;
using Microsoft.Extensions.Configuration;
using Prom.LPR.Worker.Utils;
using Prom.LPR.Worker.Models;
using System.Text.Json;
using Google.Cloud.Storage.V1;
using System.Net.Http.Headers;

namespace Prom.LPR.Worker.Executors
{
    public class LPRExecutor : BaseExecutor
    {
        private readonly IConfiguration? configuration;
        private MJobLPR? lprJob = new MJobLPR() { Message = "", JobType = "LPR" };

        private string bucket = "";
        private string lprBaseUrl = "";
        private string lprPath = "";
        private string lprAuthKey = "";

        public LPRExecutor(IConfiguration? cfg)
        {
            if (cfg == null)
            {
                Log.Error("Configuration variable is null in [LPRExector]");
            }

            if (cfg != null)
            {
                configuration = cfg;
                bucket = ConfigUtils.GetConfig(configuration, "LPRExecutor:bucket");
                lprBaseUrl = ConfigUtils.GetConfig(configuration, "LPRExecutor:lprBaseUrl");
                lprPath = ConfigUtils.GetConfig(configuration, "LPRExecutor:lprPath");
                lprAuthKey = ConfigUtils.GetConfig(configuration, "LPRExecutor:lprAuthKey");
            }
        }

        private void DeriveJob()
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            try
            {
                lprJob = JsonSerializer.Deserialize<MJobLPR>(jobParam.Message, options);
                if (lprJob != null)
                {
                    lprJob.JobId = lprJob.RefId;
                    lprJob.JobType = "LPR";
                }
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
        }

        protected override void Init()
        {
            DeriveJob();

            Log.Information($"[{lprJob?.JobType}:{lprJob?.JobId}] - Started LPR job");
            Log.Information($"[{lprJob?.JobType}:{lprJob?.JobId}] - Bucket -> [{bucket}]");
            Log.Information($"[{lprJob?.JobType}:{lprJob?.JobId}] - LPR Base Url -> [{lprBaseUrl}]");
            Log.Information($"[{lprJob?.JobType}:{lprJob?.JobId}] - LPR Path -> [{lprPath}]");
        }

        private void Final()
        {
            Log.Information($"[{lprJob?.JobType}:{lprJob?.JobId}] - Finished LPR job");
        }

        private string DownloadFile(string? gcsPath, string? objectName, string? refId) 
        {
            var ts = DateTime.Now.ToString("yyyyMMddhhmmss");
            var localPath = $"/tmp/{ts}.{refId}.jpg"; // Important to use .jpg extension, LPR needs this

            Log.Information($"[{lprJob?.JobType}:{lprJob?.JobId}] - Downloading file [{gcsPath}] to [{localPath}]");
            Log.Information($"[{lprJob?.JobType}:{lprJob?.JobId}] - Downloading object [{objectName}] to [{localPath}]");

            StorageClient storageClient = StorageClient.Create();
            using (var f = File.OpenWrite(localPath))
            {
                storageClient.DownloadObject(bucket, objectName, f);
            }

            return localPath;
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

            var productValue = new ProductInfoHeaderValue("lpr-worker", "1.0");
            requestMessage.Headers.UserAgent.Add(productValue);

            return requestMessage;
        }

        private void LPRAnalyzeFile(string imagePath)
        {
            var client = GetHttpClient();
            var requestMessage = GetRequestMessage();

            using var stream = File.OpenRead(imagePath);
            using var content = new MultipartFormDataContent
            {
                { new StreamContent(stream), "image", imagePath }
            };
            requestMessage.Content = content;

            var task = client.SendAsync(requestMessage);
            var response = task.Result;
            try
            {
                response.EnsureSuccessStatusCode();
                string responseBody = response.Content.ReadAsStringAsync().Result;

                //Use Console.WriteLine to display JSON correctly
                Console.WriteLine($"[{lprJob?.JobType}:{lprJob?.JobId}] - LPR Result -> [{responseBody}]");
            }
            catch (Exception e)
            {
                string responseBody = response.Content.ReadAsStringAsync().Result;
                Log.Error(responseBody);
                Log.Error(e.Message);
            }
        }

        protected override void ThreadExecutor()
        {
            try
            {
                var ftpPath = lprJob?.UploadPath;
                var gcsBasePath = $"gs://{bucket}/{lprJob?.UploadUser}";

                /* Replace "/ftp" with "gs://<bucket>/<user>" */
                var gcsPath = ftpPath?.Replace("/ftp", gcsBasePath);
                var objectName = ftpPath?.Replace("/ftp", $"{lprJob?.UploadUser}");

                Log.Information($"[{lprJob?.JobType}:{lprJob?.JobId}] - Company=[{lprJob?.CompanyId}]");
                Log.Information($"[{lprJob?.JobType}:{lprJob?.JobId}] - Branch=[{lprJob?.BranchId}]");
                Log.Information($"[{lprJob?.JobType}:{lprJob?.JobId}] - User=[{lprJob?.UploadUser}]");
                Log.Information($"[{lprJob?.JobType}:{lprJob?.JobId}] - FTP Path=[{lprJob?.UploadPath}]");
                Log.Information($"[{lprJob?.JobType}:{lprJob?.JobId}] - GCS Path=[{gcsPath}]");

                var localFile = DownloadFile(gcsPath, objectName, lprJob?.JobId);
                LPRAnalyzeFile(localFile);
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }

            Final();
        }
    }
}
