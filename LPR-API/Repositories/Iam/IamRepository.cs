using Prom.LPR.Api.Models;
using Prom.LPR.Api.Database;
using Microsoft.EntityFrameworkCore;

namespace Prom.LPR.Api.Repositories
{
    public class IamRepository : BaseRepository, IIamRepository
    {
        public IamRepository(DataContext ctx)
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
    }
}