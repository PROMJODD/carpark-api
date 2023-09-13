namespace Prom.LPR.Api.ViewsModels
{
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
