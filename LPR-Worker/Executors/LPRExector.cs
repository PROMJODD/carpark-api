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

            //var ts = DateTime.Now.ToString("yyyyMMddhhmm");
        }

        private void Final()
        {
            Log.Information($"[{lprJob?.JobType}:{lprJob?.JobId}] - Finished LPR job");
        }

        protected override void ThreadExecutor()
        {
            try
            {
                Log.Information($"[{lprJob?.JobType}:{lprJob?.JobId}] - Company=[{lprJob?.CompanyId}]");
                Log.Information($"[{lprJob?.JobType}:{lprJob?.JobId}] - Branch=[{lprJob?.BranchId}]");
                Log.Information($"[{lprJob?.JobType}:{lprJob?.JobId}] - User=[{lprJob?.UploadUser}]");
                Log.Information($"[{lprJob?.JobType}:{lprJob?.JobId}] - Path=[{lprJob?.UploadPath}]");

                var ftpPath = lprJob?.UploadPath;
                var gcsBasePath = $"gs://{bucket}/{lprJob?.UploadUser}";
                /* Replace "/ftp" with "gs://<bucket>/<user>" */
                var gcsPath = ftpPath?.Replace("/ftp", gcsBasePath);

                Log.Information($"[{lprJob?.JobType}:{lprJob?.JobId}] - GCS Path=[{gcsPath}]");
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }

            Final();
        }
    }
}
