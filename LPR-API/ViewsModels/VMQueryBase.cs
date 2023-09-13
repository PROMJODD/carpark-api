
namespace Prom.LPR.Api.ViewsModels
{
    public class VMQueryBase
    {
        public long? Offset { get; set; }
        public long? Limit { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        public VMQueryBase()
        {
            Limit = 50;
        }
    }
}
