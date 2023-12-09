
using System.Diagnostics.CodeAnalysis;

namespace Prom.LPR.Api.Models
{
    [ExcludeFromCodeCoverage]
    public class MStorageData
    {
        public string? StoragePath { get; set; }
        public string? PreSignedUrl { get; set; }
    }
}
