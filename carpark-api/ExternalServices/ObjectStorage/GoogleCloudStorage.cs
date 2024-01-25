using Serilog;
using Prom.LPR.Api.Models;

namespace Prom.LPR.Api.ExternalServices.ObjectStorage
{
    public class GoogleCloudStorage : IObjectStorage
    {
        private IGcsClient storageClient = new GcsClient();
        private IGcsSigner? urlSigner;
    
        public GoogleCloudStorage()
        {
        }

        public void SetStorageClient(IGcsClient sc)
        {
            //For unit testing injection
            storageClient = sc;
        }

        public void SetUrlSigner(IGcsSigner? sn)
        {
            //For unit testing injection - new GcsSigner()
            urlSigner = sn;
        }

        public IGcsSigner? GetUrlSigner()
        {
            return(urlSigner);
        }

        public MStorageData UploadFile(string localPath, string org, string bucket, string folder)
        {
            var objectName = Path.GetFileName(localPath);
            string objectPath = $"{org}/{folder}/{objectName}";
            string gcsPath = $"gs://{bucket}/{objectPath}";

            Log.Information($"Uploading file [{localPath}] to [{gcsPath}]");

            var f = File.OpenRead(localPath);
            storageClient.UploadObject(bucket, objectPath, null, f);
            f.Close();

            var url = "";
            try
            {
                url = urlSigner!.Sign(bucket, objectPath, TimeSpan.FromHours(1), HttpMethod.Get);
            }
            catch (Exception e)
            {
                Log.Error($"Unable to sign URL - [{gcsPath}]");
                Log.Error(e.Message);
            }

            var storageObj = new MStorageData() 
            {
                StoragePath = gcsPath,
                PreSignedUrl = url
            };

            return storageObj;
        }
    }
}