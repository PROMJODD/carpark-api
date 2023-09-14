using Prom.LPR.Api.Models;
using Prom.LPR.Api.Database.Repositories;
using Prom.LPR.Api.ViewsModels;

namespace Prom.LPR.Api.Services
{
    public class FileUploadedService : BaseService, IFileUploadedService
    {
        private IFileUploadedRepository? repository = null;

        public FileUploadedService(IFileUploadedRepository repo) : base()
        {
            repository = repo;
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

            return result;
        }

        public int GetFilesUploadedCount(string orgId, VMFileUploadedQuery param)
        {
            repository!.SetCustomOrgId(orgId);
            var result = repository!.GetFilesUploadedCount(param);

            return result;
        }
    }
}
