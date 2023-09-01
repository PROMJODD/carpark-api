using Serilog;
using Microsoft.Extensions.Configuration;
using Prom.LPR.Worker.Utils;
using Prom.LPR.Worker.Models;
using System.Text.Json;
using Google.Cloud.Storage.V1;

namespace Prom.LPR.Worker.Executors
{
    public class LPRExector : BaseExecutor
    {
        private readonly IConfiguration? configuration;
        private MJobLPR? lprJob = new MJobLPR() { Message = "", JobType = "LPR" };

        private string bucket = "";
        private string lprHost = "";
        private string lprPort = "";
        private string lprAuthKey = "";

        public LPRExector(IConfiguration? cfg)
        {
            if (cfg == null)
            {
                Log.Error("Configuration variable is null in [LPRExector]");
            }

            if (cfg != null)
            {
                configuration = cfg;
                bucket = ConfigUtils.GetConfig(configuration, "LPRExecutor:bucket");
                lprHost = ConfigUtils.GetConfig(configuration, "LPRExecutor:lprHost");
                lprPort = ConfigUtils.GetConfig(configuration, "LPRExecutor:lprPort");
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
        }

        private void Final()
        {
            Log.Information($"[{lprJob?.JobType}:{lprJob?.JobId}] - Finished LPR job");
        }

        private string DownloadFile(string? gcsPath, string? objectName, string? refId) 
        {
            var ts = DateTime.Now.ToString("yyyyMMddhhmmss");
            var localPath = $"/tmp/{ts}.{refId}";

            Log.Information($"[{lprJob?.JobType}:{lprJob?.JobId}] - Downloading file [{gcsPath}] to [{localPath}]");
            Log.Information($"[{lprJob?.JobType}:{lprJob?.JobId}] - Downloading object [{objectName}] to [{localPath}]");

            StorageClient storageClient = StorageClient.Create();
            using (var f = File.OpenWrite(localPath))
            {
                storageClient.DownloadObject(bucket, objectName, f);
            }

            return localPath;
        }

        protected override void ThreadExecutor()
        {
            try
            {
                var ftpPath = lprJob?.UploadPath;
                var gcsBasePath = $"gs://{bucket}/{lprJob?.UploadUser}";

                /* Replace "/ftp" with "gs://<bucket>/<user>" */
                var gcsPath = ftpPath?.Replace("/ftp", gcsBasePath);
                var objectName = ftpPath?.Replace("/ftp/", "");

                Log.Information($"[{lprJob?.JobType}:{lprJob?.JobId}] - Company=[{lprJob?.CompanyId}]");
                Log.Information($"[{lprJob?.JobType}:{lprJob?.JobId}] - Branch=[{lprJob?.BranchId}]");
                Log.Information($"[{lprJob?.JobType}:{lprJob?.JobId}] - User=[{lprJob?.UploadUser}]");
                Log.Information($"[{lprJob?.JobType}:{lprJob?.JobId}] - Path=[{lprJob?.UploadPath}]");
                Log.Information($"[{lprJob?.JobType}:{lprJob?.JobId}] - GCS Path=[{gcsPath}]");

                var localFile = DownloadFile(gcsPath, objectName, lprJob?.JobId);
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }

            Final();
        }
    }
}
