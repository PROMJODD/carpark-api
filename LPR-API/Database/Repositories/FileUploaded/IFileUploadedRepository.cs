using Prom.LPR.Api.Models;
using Prom.LPR.Api.ViewsModels;

namespace Prom.LPR.Api.Database.Repositories
{
    public interface IFileUploadedRepository
    {
        public void SetCustomOrgId(string customOrgId);
        public MFileUploaded AddFileUploaded(MFileUploaded file);
        public IEnumerable<MFileUploaded> GetFilesUploaded(VMFileUploadedQuery param);
    }
}
