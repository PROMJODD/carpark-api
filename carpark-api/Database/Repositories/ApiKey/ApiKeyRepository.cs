using Prom.LPR.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Prom.LPR.Api.Database.Repositories
{
    public class ApiKeyRepository : BaseRepository, IApiKeyRepository
    {
        public ApiKeyRepository(IDataContext ctx)
        {
            context = ctx;
        }

        public Task<MApiKey> GetApiKey(string apiKey)
        {
            var result = context!.ApiKeys!.Where(x => x.OrgId!.Equals(orgId) && x.ApiKey!.Equals(apiKey)).FirstOrDefaultAsync();
            return result!;
        }

        public MApiKey AddApiKey(MApiKey apiKey)
        {
            apiKey.KeyId = Guid.NewGuid();
            apiKey.KeyCreatedDate = DateTime.UtcNow;
            apiKey.OrgId = orgId;

            context!.ApiKeys!.Add(apiKey);
            context.SaveChanges();

            return apiKey;
        }

        public MApiKey? DeleteApiKeyById(string keyId)
        {
            Guid id = Guid.Parse(keyId);

            var r = context!.ApiKeys!.Where(x => x.OrgId!.Equals(orgId) && x.KeyId.Equals(id)).FirstOrDefault();
            if (r != null)
            {
                context!.ApiKeys!.Remove(r);
                context.SaveChanges();
            }

            return r;
        }

        public IEnumerable<MApiKey> GetApiKeys()
        {
            var arr = context!.ApiKeys!.Where(x => x.OrgId!.Equals(orgId)).ToList();
            return arr;
        }
    }
}