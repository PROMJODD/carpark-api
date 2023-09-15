using Prom.LPR.Api.Models;

namespace Prom.LPR.Api.ModelsViews
{
    public class MVOrganizationUser
    {
        public string? Status { get; set; }
        public string? Description { get; set; }
        public MOrganizationUser? OrgUser { get; set; }
        public MUser? User { get; set; }
    }
}
