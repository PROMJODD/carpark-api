using Prom.LPR.Api.Models;
using Prom.LPR.Api.ModelsViews;

namespace Prom.LPR.Api.Services
{
    public interface IApiKeyService
    {
        public Task<MApiKey> GetApiKey(string orgId, string apiKey);
        public MVApiKey VerifyApiKey(string orgId, string apiKey);
        public MVApiKey? AddApiKey(string orgId, MApiKey apiKey);
        public MVApiKey? DeleteApiKeyById(string orgId, string keyId);
        public IEnumerable<MApiKey> GetApiKeys(string orgId);
    }
}
