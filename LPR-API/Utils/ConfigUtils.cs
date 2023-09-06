
namespace Prom.LPR.Api.Utils
{
    public class ConfigUtils
    {
        public static string GetConfig(IConfiguration cfg, string key)
        {
            var v = "";

            var envKey = key.Replace(":", "__");
            var envVar = Environment.GetEnvironmentVariable(envKey);

            if (envVar == null)
            {
                if (cfg != null)
                {            
                    var u = cfg[key];
                    v = String.IsNullOrWhiteSpace(u) ? "" : u;
                }
            }
            else
            {
                v = envVar;
            }

            return v;
        }
    }
}
