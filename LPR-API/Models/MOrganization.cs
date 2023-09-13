using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using IndexAttribute = Microsoft.EntityFrameworkCore.IndexAttribute;

namespace Prom.LPR.Api.Models
{
    [Table("Organizations")]
    [Index(nameof(OrgCustomId), IsUnique = true)]
    public class MOrganization
    {
        [Key]
        [Column("org_id")]
        public Guid? OrgId { get; set; }
    
        [Column("org_custom_id")]
        public string? OrgCustomId { get; set; }

        [Column("org_name")]
        public string? OrgName { get; set; }

        [Column("org_description")]
        public string? OrgDescription { get; set; }

        [Column("org_created_date")]
        public DateTime? OrgCreatedDate { get; set; }
    }
}
