using Serilog;
using Microsoft.Extensions.Configuration;
using Prom.LPR.Worker.Utils;
using Prom.LPR.Worker.Models;
using System.Text.Json;
using Google.Cloud.Storage.V1;
using System.Net.Http.Headers;

namespace Prom.LPR.Worker.Executors
{
    public class LprExecutor : BaseExecutor
    {
        private MJobLpr? lprJob = new MJobLpr() { Message = "", JobType = "LPR" };

        private readonly string bucket;
        private readonly string lprBaseUrl;
        private readonly string lprPath;
        private readonly string lprAuthKey;

        public LprExecutor(IConfiguration? cfg)
        {
            if (cfg == null)
            {
                Log.Error("Configuration variable is null in [LPRExector]");
            }

            bucket = "";
            lprBaseUrl = "";
            lprPath = "";
            lprAuthKey = "";

            if (cfg != null)
            {
                bucket = ConfigUtils.GetConfig(cfg, "LPRExecutor:bucket");
                lprBaseUrl = ConfigUtils.GetConfig(cfg, "LPRExecutor:lprBaseUrl");
                lprPath = ConfigUtils.GetConfig(cfg, "LPRExecutor:lprPath");
                lprAuthKey = ConfigUtils.GetConfig(cfg, "LPRExecutor:lprAuthKey");
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
                lprJob = JsonSerializer.Deserialize<MJobLpr>(jobParam.Message, options);
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

        private string DownloadFile(string? gcsPath, string? objectName) 
        {
            var tmpFile = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            var localPath = $"{tmpFile}.jpg";

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
                var objectName = ftpPath?.Replace("/ftp", lprJob!.UploadUser);

                Log.Information($"[{lprJob?.JobType}:{lprJob?.JobId}] - Company=[{lprJob?.CompanyId}]");
                Log.Information($"[{lprJob?.JobType}:{lprJob?.JobId}] - Branch=[{lprJob?.BranchId}]");
                Log.Information($"[{lprJob?.JobType}:{lprJob?.JobId}] - User=[{lprJob?.UploadUser}]");
                Log.Information($"[{lprJob?.JobType}:{lprJob?.JobId}] - FTP Path=[{lprJob?.UploadPath}]");
                Log.Information($"[{lprJob?.JobType}:{lprJob?.JobId}] - GCS Path=[{gcsPath}]");

                var localFile = DownloadFile(gcsPath, objectName);
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
