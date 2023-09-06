
namespace Prom.LPR.Api.Models
{
    public class MVehicle : BaseModel
    {
        public string? License { get; set; }
        public string? Province { get; set; }
        public string? Brand { get; set; }
        public string? Class { get; set; }
        public string? Color { get; set; }
        public int? Remaining { get; set; }
    }
}
