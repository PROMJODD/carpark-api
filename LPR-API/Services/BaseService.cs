
namespace Prom.LPR.Api.Services
{
    public class BaseService
    {
        protected string orgId = "";
    
        public BaseService()
        {
        }

        public void SetCustomOrgId(string customOrgId)
        {
            orgId = customOrgId;
        }
    }
}
