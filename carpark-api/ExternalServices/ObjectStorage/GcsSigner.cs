using Google.Cloud.Storage.V1;
using Google.Apis.Auth.OAuth2;
using System.Diagnostics.CodeAnalysis;

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
            //gcsPath --> gcs://<bucket>/<objectPath>
            //TODO : Capture bucket and objectPath using regex

            if (gcsPath == null)
            {
                return "";
            }

            //var bucket = "";
            //var objectPath = "";

            //var url = urlSigner!.Sign(bucket, objectPath, TimeSpan.FromHours(hour), HttpMethod.Get);
            var url = "https://this-is-presigned-url/aaa.jpg";

            return url; 
        }
    }
}
