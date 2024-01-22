using Serilog;
using Prom.LPR.Api.Models;
using Prom.LPR.Api.Database.Repositories;
using Prom.LPR.Api.ViewsModels;
using Prom.LPR.Api.ExternalServices.Recognition;
using Prom.LPR.Api.ExternalServices.ObjectStorage;
using Prom.LPR.Api.Utils;
using Prom.LPR.Api.ExternalServices.Cache;

namespace Prom.LPR.Api.Services
{
    public class FileUploadedService : BaseService, IFileUploadedService
    {
        private readonly IFileUploadedRepository? repository = null;
        private readonly IImageAnalyzer? analyzer;
        private readonly IObjectStorage? gcs;
        private readonly string imagesBucket;
        private readonly ICache cached;

        public FileUploadedService(
            IFileUploadedRepository repo,
            IObjectStorage objStorage,
            IConfiguration cfg,
            IGcsSigner signer,
            ICache cache,
            IImageAnalyzer anlzr) : base()
        {
            repository = repo;
            analyzer = anlzr;

            gcs = objStorage;
            gcs.SetUrlSigner(signer);
            imagesBucket = ConfigUtils.GetConfig(cfg, "LPR:bucket");
            cached = cache;
        }

        public MFileUploaded AddFileUploaded(string orgId, MFileUploaded file)
        {
            repository!.SetCustomOrgId(orgId);
            var result = repository!.AddFileUploaded(file);

            return result;
        }

        public IEnumerable<MFileUploaded> GetFilesUploaded(string orgId, VMFileUploadedQuery param)
        {
            repository!.SetCustomOrgId(orgId);
            var result = repository!.GetFilesUploaded(param);

            var signer = gcs!.GetUrlSigner();
            var domain = "gcs_uploaded_file";

            foreach (var f in result)
            {
                if ((f.StoragePath != null) && !f.StoragePath.Equals(""))
                {
                    var presignedUrl = cached.GetValue<string>(domain, f.StoragePath);
                    if (presignedUrl == null)
                    {
                        //Not found in cache
                        //Expire every 25 hours but cached will expire on every 24 hours

                        presignedUrl = signer!.Sign(f.StoragePath, 25);
                        cached.SetValue(domain, f.StoragePath, presignedUrl, 24 * 60);
                    }

                    f.PresignedUrl = presignedUrl;
                }
            }

            return result;
        }

        public int GetFilesUploadedCount(string orgId, VMFileUploadedQuery param)
        {
            repository!.SetCustomOrgId(orgId);
            var result = repository!.GetFilesUploadedCount(param);

            return result;
        }

        private static string GetContextValue(HttpContext context, string key)
        {
            bool t = context.Items.TryGetValue(key, out object? e);
            if (t)
            {
                var value = e as string;
                return value!;
            }

            return "";
        }

        private void AddRecord(string id, MKafkaMessage data, string fname, HttpContext context)
        {
            FileInfo fi = new FileInfo(fname);
            var m = new MFileUploaded()
            {
                OrgId = id,

                IdentityType = GetContextValue(context, "Temp-Identity-Type"),
                UploaderId = GetContextValue(context, "Temp-Identity-Id"),
                UploadedApi = GetContextValue(context, "Temp-API-Called"),

                StoragePath = data!.StorageData!.StoragePath,
                RecognitionStatus = data!.LprData!.Status.ToString(),
                RecognitionMessage = data!.LprData!.Message,
                VehicleLicense = data!.LprData!.Data!.License,
                VehicleProvince = data!.LprData!.Data!.Province,
                VehicleBrand = data!.LprData!.Data!.VehBrand,
                VehicleClass = data!.LprData!.Data!.VehClass,
                VehicleColor = data!.LprData!.Data!.VehColor,
                QuotaLeft = data!.LprData!.Data!.Remaining,
                FileSize = fi.Length,
            };

            AddFileUploaded(id, m);
        }

        public MLprResponse UploadFile(string orgId, MImageUploaded data, HttpContext context)
        {
            var resp = new MLprResponse() { Status = "OK", Description = "Success" };

            var image = data.Image;
            if (image == null)
            {
                resp.Status = "NOTFOUND";
                resp.Description = $"Uploaded file not found!!!";
                return resp;
            }

            string ext = Path.GetExtension(image.FileName);
            var tmpFile = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            tmpFile = $"{tmpFile}{ext}";

            using (var fileStream = new FileStream(tmpFile, FileMode.Create))
            {
                image.CopyTo(fileStream);
            }

            Log.Information($"Uploaded file [{image.FileName}], saved to [{tmpFile}]");
            var lprObj = analyzer!.AnalyzeFile<MLprResult>(tmpFile);

            var dateStamp = DateTime.Now.ToString("yyyyMMddhh");
            var storageObj = gcs!.UploadFile(tmpFile, orgId, imagesBucket, dateStamp);

            var msg = new MKafkaMessage() 
            {
                LprData = lprObj,
                StorageData = storageObj,
                HttpRequestHeader = context.Request.Headers
            };
            AddRecord(orgId, msg, tmpFile, context);

            resp.LprData = lprObj;
            resp.StorageData = storageObj;
        
            return resp;
        }
    }
}
