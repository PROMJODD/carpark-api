using Prom.LPR.Api.ExternalServices.ObjectStorage;

namespace Prom.LPR.Test.Api.Services
{
    public class GcsSignerMocked : IGcsSigner
    {
        public GcsSignerMocked()
        {
        }

        public string Sign(string bucket, string objectPath, TimeSpan ts, HttpMethod method)
        {
            return "";
        }

        public string Sign(string? gcsPath, int hour)
        {
            return "";
        }
    }
}
