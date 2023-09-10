using Prom.LPR.Api.Models;
using Prom.LPR.Api.ModelsViews;

namespace Prom.LPR.Api.Services
{
    public interface IIamService
    {
        public Task<MApiKey> GetApiKey(string orgId, string apiKey);
        public MVApiKeyVerify VerifyApiKey(string orgId, string apiKey);
    }
}
