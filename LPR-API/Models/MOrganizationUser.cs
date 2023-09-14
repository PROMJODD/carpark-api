using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using IndexAttribute = Microsoft.EntityFrameworkCore.IndexAttribute;

namespace Prom.LPR.Api.Models
{
    [Table("OrganizationsUsers")]
    [Index(nameof(OrgCustomId), IsUnique = false)]
    [Index(nameof(OrgCustomId), nameof(UserId), IsUnique = true, Name = "OrgUser_Unique1")]
    public class MOrganizationUser
    {
        [Key]
        [Column("org_user_id")]
        public Guid? OrgUserId { get; set; }

        [Column("org_custom_id")]
        public string? OrgCustomId { get; set; }

        [Column("user_id")]
        public string? UserId { get; set; }

        [Column("user_name")]
        public string? UserName { get; set; }

        [Column("created_date")]
        public DateTime? CreatedDate { get; set; }

        [Column("roles_list")]
        public string? RolesList { get; set; }

        public MOrganizationUser()
        {
            OrgUserId = Guid.NewGuid();
            CreatedDate = DateTime.UtcNow;
            RolesList = "";
        }
    }
}
