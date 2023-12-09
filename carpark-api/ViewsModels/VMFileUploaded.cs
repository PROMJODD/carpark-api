using System.Diagnostics.CodeAnalysis;

namespace Prom.LPR.Api.ViewsModels
{
    [ExcludeFromCodeCoverage]
    public class VMFileUploadedQuery : VMQueryBase
    {
        public string? VehicleLicense { get; set; }
        public string? VehicleProvince { get; set; }
        public string? VehicleBrand { get; set; }
        public string? VehicleClass { get; set; }
        public string? VehicleColor { get; set; }
        public string? UploadedApi { get; set; }
    }
}
