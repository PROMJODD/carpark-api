
namespace Prom.LPR.Worker.Models
{
    public class BaseModel
    {
        public required string Id { get; set; }

        public DateTime ModifiedDtm { get; set; }
        
        public DateTime CreatedDtm { get; set; }
    }
}
