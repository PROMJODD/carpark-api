
using System.Diagnostics.CodeAnalysis;

namespace Prom.LPR.Api.Models
{
    [ExcludeFromCodeCoverage]
    public class MLPRResponse
    {
        public MStorageData? StorageData { get; set; }
        public MLPRResult? LprData { get; set; }
    }
}
