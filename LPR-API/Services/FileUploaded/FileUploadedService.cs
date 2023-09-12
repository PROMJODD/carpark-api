using Prom.LPR.Api.Models;
using Prom.LPR.Api.Database.Repositories;

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
    }
}
