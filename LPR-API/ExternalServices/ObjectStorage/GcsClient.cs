using Google.Cloud.Storage.V1;
using System.Diagnostics.CodeAnalysis;
using Object = Google.Apis.Storage.v1.Data.Object;

namespace Prom.LPR.Api.ExternalServices.ObjectStorage
{
    [ExcludeFromCodeCoverage]
    public class GcsClient : IGcsClient
    {
        private StorageClient storageClient = StorageClient.Create();

        public GcsClient()
        {
        }

        public Object UploadObject(string bucket, string objectName, string? contentType, Stream source)
        {
            var o = storageClient.UploadObject(bucket, objectName, contentType, source);
            return o;
        }
    }
}
