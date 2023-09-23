namespace Prom.LPR.Api.ExternalServices.ObjectStorage
{
    public interface IGcsSigner
    {
        public string Sign(string bucket, string objectPath, TimeSpan ts, HttpMethod method);
    }
}
