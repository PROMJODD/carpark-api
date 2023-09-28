
using System.Text.RegularExpressions;

namespace Prom.LPR.Api.Utils
{
    public static class ServiceUtils
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

        public static string GetOrgId(HttpRequest request)
        {
            var pattern = @"^\/api\/(.+)\/org\/(.+)\/action\/(.+)$";
            var path = request.Path;
            MatchCollection matches = Regex.Matches(path, pattern, RegexOptions.None, TimeSpan.FromMilliseconds(100));

            var orgId = matches[0].Groups[2].Value;

            return orgId;
        }
    }
}
