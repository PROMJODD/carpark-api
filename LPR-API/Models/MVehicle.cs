
using System.Diagnostics.CodeAnalysis;

namespace Prom.LPR.Api.Models
{
    [ExcludeFromCodeCoverage]
    public class MVehicle
    {
        public string? _Id { get; set; }
        public string? License { get; set; }
        public string? Province { get; set; }
        public string? VehBrand { get; set; }
        public string? VehClass { get; set; }
        public string? VehColor { get; set; }
        public int? Remaining { get; set; }
    }
}
