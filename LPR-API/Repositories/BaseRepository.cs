using Prom.LPR.Api.Database;

namespace Prom.LPR.Api.Repositories
{
    public class BaseRepository
    {
        private const string RESERVE_ORG_ID = "axxxxnotdefinedxxxxxxa";

        protected DataContext? context;
        protected string orgId = RESERVE_ORG_ID;

        public string GetReserveCustomOrgId()
        {
            return RESERVE_ORG_ID;
        }

        public void SetCustomOrgId(string customOrgId)
        {
            orgId = customOrgId;
        }
    }
}
