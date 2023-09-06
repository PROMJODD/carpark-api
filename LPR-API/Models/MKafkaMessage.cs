
namespace Prom.LPR.Api.Models
{
    public class MKafkaMessage
    {
        public MStorageData? StorageData { get; set; }
        public MLPRResult? LprData { get; set; }
    }
}
