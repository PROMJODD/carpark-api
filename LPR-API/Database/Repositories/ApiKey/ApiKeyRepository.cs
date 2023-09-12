using Prom.LPR.Api.Models;
using Prom.LPR.Api.Database;
using Microsoft.EntityFrameworkCore;

namespace Prom.LPR.Api.Database.Repositories
{
    public class ApiKeyRepository : BaseRepository, IApiKeyRepository
    {
        public ApiKeyRepository(DataContext ctx)
        {
            context = ctx;
        }

        public Task<MApiKey> GetApiKey(string apiKey)
        {
            try
            {
                var result = context!.ApiKeys!.Where(x => x.OrgId!.Equals(orgId) && x.ApiKey!.Equals(apiKey)).FirstOrDefaultAsync();
                return result!;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public MApiKey AddApiKey(MApiKey apiKey)
        {
            try
            {
                apiKey.KeyId = Guid.NewGuid();
                apiKey.KeyCreatedDate = DateTime.UtcNow;
                apiKey.OrgId = orgId;

                context!.ApiKeys!.AddAsync(apiKey);
                context.SaveChanges();
            }
            catch (Exception)
            {
                throw;
            }

            return apiKey;
        }

        public MApiKey? DeleteApiKeyById(string keyId)
        {
            try
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
            catch (Exception)
            {
                throw;
            }
        }

        public IEnumerable<MApiKey> GetApiKeys()
        {
            try
            {
                var arr = context!.ApiKeys!.Where(x => x.OrgId!.Equals(orgId)).ToList();
                return arr;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}