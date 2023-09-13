using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using IndexAttribute = Microsoft.EntityFrameworkCore.IndexAttribute;

namespace Prom.LPR.Api.Models
{
    [Table("Roles")]
    [Index(nameof(RoleName), IsUnique = true)]
    public class MRole
    {
        [Key]
        [Column("role_id")]
        public Guid? RoleId { get; set; }
    
        [Column("role_name")]
        public string? RoleName { get; set; }

        [Column("role_description")]
        public string? RoleDescription { get; set; }
    
        [Column("role_created_date")]
        public DateTime? RoleCreatedDate { get; set; }

        [Column("role_definition")]
        public string? RoleDefinition { get; set; }

        [Column("role_level")]
        public string RoleLevel { get; set; }

        public MRole()
        {
            RoleId = Guid.NewGuid();
            RoleCreatedDate = DateTime.UtcNow;
            RoleLevel = "";
        }
    }
}
