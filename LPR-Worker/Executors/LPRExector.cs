using Serilog;
using Microsoft.Extensions.Configuration;
using Prom.LPR.Worker.Utils;

namespace Prom.LPR.Worker.Executors
{
    public class LPRExector : BaseExecutor
    {
        private readonly IConfiguration? configuration;

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

        protected override void Init()
        {
            Log.Information($"[{jobParam.Type}:{jobParam.JobId}] - Started LPR job");
            Log.Information($"[{jobParam.Type}:{jobParam.JobId}] - Bucket -> [{bucket}]");

            var ts = DateTime.Now.ToString("yyyyMMddhhmm");
        }

        private void Final()
        {
            Log.Information($"[{jobParam.Type}:{jobParam.JobId}] - Finished LPR job");
        }

        protected override void ThreadExecutor()
        {
            //Do LPR logic here
            Log.Information(jobParam.Message);

            Final();
        }
    }
}
