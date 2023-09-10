using Prom.LPR.Api.Models;

namespace Prom.LPR.Api.Repositories
{
    public interface IIamRepository
    {
        public void SetCustomOrgId(string customOrgId);
        public Task<MApiKey> GetApiKey(string apiKey);
    }
}
