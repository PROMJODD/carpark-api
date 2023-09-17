
using System.Diagnostics.CodeAnalysis;

namespace Prom.LPR.Api.Models
{
    [ExcludeFromCodeCoverage]
    public class MKafkaMessage
    {
        public IHeaderDictionary? HttpRequestHeader { get; set; }
        public MStorageData? StorageData { get; set; }
        public MLPRResult? LprData { get; set; }
    }
}
