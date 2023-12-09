using System.Diagnostics.CodeAnalysis;

namespace Prom.LPR.Api.Models
{
    [ExcludeFromCodeCoverage]
    public class MImageUploaded
    {
        public string? Name { get; set; }
        public IFormFile? Image { get; set; }
    }
}
