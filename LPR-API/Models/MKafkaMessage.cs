
namespace Prom.LPR.Api.Models
{
    public class MKafkaMessage
    {
        public IHeaderDictionary? HttpRequestHeader { get; set; }
        public MStorageData? StorageData { get; set; }
        public MLPRResult? LprData { get; set; }
    }
}
