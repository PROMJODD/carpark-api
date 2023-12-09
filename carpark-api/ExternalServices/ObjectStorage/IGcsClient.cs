using Object = Google.Apis.Storage.v1.Data.Object;

namespace Prom.LPR.Api.ExternalServices.ObjectStorage
{
    public interface IGcsClient
    {
        Object UploadObject(string bucket, string objectName, string? contentType, Stream source);
    }
}
