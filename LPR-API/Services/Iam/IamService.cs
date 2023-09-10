using Prom.LPR.Api.Models;
using Prom.LPR.Api.ModelsViews;
using Prom.LPR.Api.Repositories;

namespace Prom.LPR.Api.Services
{
    public class IamService : BaseService, IIamService
    {
        private IIamRepository? repository = null;

        public IamService(IIamRepository repo) : base()
        {
            repository = repo;
        }

        public Task<MApiKey> GetApiKey(string orgId, string apiKey)
        {
            repository!.SetCustomOrgId(orgId);
            var result = repository!.GetApiKey(apiKey);

            return result;
        }

        public MVApiKeyVerify VerifyApiKey(string orgId, string apiKey)
        {
            repository!.SetCustomOrgId(orgId);
            var m = repository!.GetApiKey(apiKey).Result;

            var status = "OK";
            var description = "SUCCESS";

            if (m == null)
            {
                status = "NOTFOUND";
                description = $"API key not found for the organization [{orgId}]";
            }
            else if ((m.KeyExpiredDate != null) && (m.KeyExpiredDate < DateTime.Now))
            {
                status = "EXPIRED";
                description = $"API key for the organization is expire [{orgId}] since [{m.KeyExpiredDate}]";
            }

            var mv = new MVApiKeyVerify() 
            {
                ApiKey = m,
                Status = status,
                Description = description,
            };

            return mv;
        }
    }
}
