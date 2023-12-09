namespace Prom.LPR.Api.Database.Repositories
{
    public class BaseRepository
    {
        private const string RESERVE_ORG_ID = "axxxxnotdefinedxxxxxxa";

        protected IDataContext? context;
        protected string orgId = RESERVE_ORG_ID;

        public void SetCustomOrgId(string customOrgId)
        {
            orgId = customOrgId;
        }
    }
}
