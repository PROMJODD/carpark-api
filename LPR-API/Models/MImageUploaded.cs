using System.Diagnostics.CodeAnalysis;

namespace Prom.LPR.Api.Models
{
    public class MImageUploaded
    {
        [ExcludeFromCodeCoverage]
        public string? Name { get; set; }
        public IFormFile? Image { get; set; }
    }
}
