
using System.Diagnostics.CodeAnalysis;

namespace Prom.LPR.Api.Models
{
    [ExcludeFromCodeCoverage]
    public class MLPRResponse
    {
        public string? Status { get; set; }
        public string? Description { get; set; }
        public MStorageData? StorageData { get; set; }
        public MLPRResult? LprData { get; set; }
    }
}
