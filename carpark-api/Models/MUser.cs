using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using IndexAttribute = Microsoft.EntityFrameworkCore.IndexAttribute;
using System.Diagnostics.CodeAnalysis;

namespace Prom.LPR.Api.Models
{
    [ExcludeFromCodeCoverage]
    [Table("Users")]
    [Index(nameof(UserName), IsUnique = true)]
    [Index(nameof(UserEmail), IsUnique = true)]
    public class MUser
    {
        [Key]
        [Column("user_id")]
        public Guid? UserId { get; set; }
    
        [Column("user_name")]
        public string? UserName { get; set; }

        [Column("user_email")]
        public string? UserEmail { get; set; }

        [Column("user_created_date")]
        public DateTime? UserCreatedDate { get; set; }

        public MUser()
        {
            UserId = Guid.NewGuid();
            UserCreatedDate = DateTime.UtcNow;
        }
    }
}
