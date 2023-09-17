using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using IndexAttribute = Microsoft.EntityFrameworkCore.IndexAttribute;
using System.Diagnostics.CodeAnalysis;

namespace Prom.LPR.Api.Models
{
    [ExcludeFromCodeCoverage]
    [Table("FileUploaded")]
    [Index(nameof(StoragePath), IsUnique = true)]
    [Index(nameof(UploadedApi), IsUnique = false)]
    [Index(nameof(VehicleLicense), IsUnique = false)]
    [Index(nameof(VehicleBrand), IsUnique = false)]
    [Index(nameof(VehicleProvince), IsUnique = false)]
    [Index(nameof(VehicleClass), IsUnique = false)]
    public class MFileUploaded
    {
        [Key]
        [Column("file_id")]
        public Guid FileId { get; set; }
    
        [Column("org_id")]
        public string? OrgId { get; set; }

        [Column("identity_type")]
        public string? IdentityType { get; set; }

        [Column("uploader_id")]
        public string? UploaderId { get; set; }

        [Column("storage_path")]
        public string? StoragePath { get; set; }

        [Column("recognition_status")]
        public string? RecognitionStatus { get; set; }

        [Column("recognition_message")]
        public string? RecognitionMessage { get; set; }

        [Column("vehicle_license")]
        public string? VehicleLicense { get; set; }

        [Column("vehicle_province")]
        public string? VehicleProvince { get; set; }

        [Column("vehicle_brand")]
        public string? VehicleBrand { get; set; }

        [Column("vehicle_class")]
        public string? VehicleClass { get; set; }

        [Column("vehicle_color")]
        public string? VehicleColor { get; set; }

        [Column("quota_left")]
        public long? QuotaLeft { get; set; }

        [Column("uploaded_date")]
        public DateTime? UploadedDate { get; set; }

        [Column("uploaded_api")]
        public string? UploadedApi { get; set; }

        [Column("file_size")]
        public long? FileSize { get; set; }

        public MFileUploaded()
        {
            FileId = Guid.NewGuid();
            UploadedDate = DateTime.UtcNow;
            QuotaLeft = 0;
        }
    }
}
