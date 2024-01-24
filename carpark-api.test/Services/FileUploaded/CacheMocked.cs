using System.Collections;
using Prom.LPR.Api.ExternalServices.Cache;

namespace Prom.LPR.Test.Api.Services
{
    public class CacheMocked : ICache
    {
        private Hashtable cache = new Hashtable
        {
            {"lic-1", "gs://bucket/lic-1.pjg"},
            {"lic-2", "gs://bucket/lic-2.pjg"},
        };

        public CacheMocked()
        {
        }

        public T? GetValue<T>(string domain, string key)
        {
            var value = cache[key];
            return (T?) value;
        }

        public void SetValue<T>(string domain, string key, T data, int lifeTimeMin)
        {
            cache[key] = data;
        }
    }
}
