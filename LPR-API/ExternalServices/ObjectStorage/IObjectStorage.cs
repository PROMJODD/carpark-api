using Prom.LPR.Api.Models;

namespace Prom.LPR.Api.ExternalServices.ObjectStorage
{
    public interface IObjectStorage
    {
        public MStorageData UploadFile(string localPath, string org, string bucket, string folder);
    }
}
