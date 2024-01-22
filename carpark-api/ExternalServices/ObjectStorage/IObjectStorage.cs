using Prom.LPR.Api.Models;

namespace Prom.LPR.Api.ExternalServices.ObjectStorage
{
    public interface IObjectStorage
    {
        public MStorageData UploadFile(string localPath, string org, string bucket, string folder);
        public void SetUrlSigner(IGcsSigner? sn);
        public IGcsSigner? GetUrlSigner();
    }
}
