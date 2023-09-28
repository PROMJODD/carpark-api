using Google.Cloud.Storage.V1;
using Google.Apis.Auth.OAuth2;
using System.Diagnostics.CodeAnalysis;

namespace Prom.LPR.Api.ExternalServices.ObjectStorage
{
    [ExcludeFromCodeCoverage]
    public class GcsSigner : IGcsSigner
    {
        private readonly UrlSigner urlSigner;

        public GcsSigner()
        {
            urlSigner = UrlSigner.FromCredential(GoogleCredential.GetApplicationDefault());
        }

        public string Sign(string bucket, string objectPath, TimeSpan ts, HttpMethod method)
        {
            var url = urlSigner.Sign(bucket, objectPath, TimeSpan.FromHours(1), HttpMethod.Get);
            return url;
        }
    }
}
