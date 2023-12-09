
using System.Diagnostics.CodeAnalysis;

namespace Prom.LPR.Api.Models
{
    [ExcludeFromCodeCoverage]
    public class MLprResult
    {
        public int? Status { get; set; }
        public string? Message { get; set; }
        public MVehicle? Data { get; set; }
    }
}
