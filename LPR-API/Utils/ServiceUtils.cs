
namespace Prom.LPR.Api.Utils
{
    public class ServiceUtils
    {
        public static bool IsGuidValid(string guid)
        {
            try
            {
                Guid id = Guid.Parse(guid);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
