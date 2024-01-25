
namespace Prom.LPR.Api.ExternalServices.Cache
{
    public interface ICache
    {
        public T? GetValue<T>(string domain, string key);
        public void SetValue<T>(string domain, string key, T data, int lifeTimeMin);
    }
}
