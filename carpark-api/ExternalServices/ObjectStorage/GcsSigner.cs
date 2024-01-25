using Google.Cloud.Storage.V1;
using Google.Apis.Auth.OAuth2;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace Prom.LPR.Api.ExternalServices.ObjectStorage
{
    [ExcludeFromCodeCoverage]
    public class GcsSigner : IGcsSigner
    {
        private readonly UrlSigner? urlSigner;

        public GcsSigner()
        {
            try
            {
                urlSigner = UrlSigner.FromCredential(GoogleCredential.GetApplicationDefault());
            }
            catch
            {
                urlSigner = null;
            }
        }

        public string Sign(string bucket, string objectPath, TimeSpan ts, HttpMethod method)
        {
            var url = urlSigner!.Sign(bucket, objectPath, TimeSpan.FromHours(1), HttpMethod.Get);
            return url;
        }

        public string Sign(string? gcsPath, int hour)
        {
            if (gcsPath == null)
            {
                return "";
            }

            var gcsPattern = @"^gs:\/\/(.+?)\/(.+)$";
            var matches = Regex.Matches(gcsPath, gcsPattern, RegexOptions.None, TimeSpan.FromMilliseconds(100));

            var bucket = matches[0].Groups[1].Value;
            var objectPath = matches[0].Groups[2].Value;
            var url = urlSigner!.Sign(bucket, objectPath, TimeSpan.FromHours(hour), HttpMethod.Get);

            return url; 
        }
    }
}
