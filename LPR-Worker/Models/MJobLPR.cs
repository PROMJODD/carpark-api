
namespace Prom.LPR.Worker.Models
{
    public class MJobLPR : MJob
    {
        public string? JobType { get; set; }
        public string? BranchId { get; set; }
        public string? UploadPath { get; set; }
        public string? RefId { get; set; }
        public string? CompanyId { get; set; }
        public string? EquipmentId { get; set; }
        public string? UploadUser { get; set; }
        public string? UploadTimeMs { get; set; }
        public long UploadSize { get; set; }
        public long StartDtm { get; set; }
    }
}
