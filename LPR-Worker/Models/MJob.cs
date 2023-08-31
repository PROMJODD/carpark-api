
namespace Prom.LPR.Worker.Models
{
    public class MJob : BaseModel
    {
        public DateTime JobDate { get; set; }
        public string? JobId { get; set; }
        public string? Type { get; set; }

        public string? LPRUploadPath { get; set; }
    }
}
