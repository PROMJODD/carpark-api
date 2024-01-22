using System.Diagnostics.CodeAnalysis;
using Enyim.Caching;
using Enyim.Caching.Configuration;
using Prom.LPR.Api.Utils;
using Serilog.Sinks.Syslog;

namespace Prom.LPR.Api.ExternalServices.Cache;

[ExcludeFromCodeCoverage]
public class MemCached : ICache
{
    private readonly IMemcachedClient cacheClient;

    public MemCached(IConfiguration cfg)
    {
        var config = new MemcachedClientConfiguration(null, new MemcachedClientOptions());

        var host = ConfigUtils.GetConfig(cfg, "Memcached:host");
        var portStr = ConfigUtils.GetConfig(cfg, "Memcached:port");

        config.AddServer(host, portStr.ToInt());
        cacheClient = new MemcachedClient(null, config);
    }

    public T? GetValue<T>(string domain, string key)
    {
        var lookupKey = $"{domain}:{key}";
        var value = cacheClient.Get<T>(lookupKey);

        return value;
    }

    public void SetValue<T>(string domain, string key, T data, int lifeTimeMin)
    {
        var lookupKey = $"{domain}:{key}";

        var cachedSecond = lifeTimeMin * 60;
        cacheClient.Set(lookupKey, data, cachedSecond);
    }
}
