
namespace Prom.LPR.Api.Models
{
    public class MKafkaMessage : BaseModel
    {
        public string? StoragePath { get; set; }
        public MLPRResult? LprData { get; set; }
    }
}
