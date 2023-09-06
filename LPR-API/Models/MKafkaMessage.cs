
namespace Prom.LPR.Api.Models
{
    public class MKafkaMessage
    {
        public string? StoragePath { get; set; }
        public MLPRResult? LprData { get; set; }
    }
}
