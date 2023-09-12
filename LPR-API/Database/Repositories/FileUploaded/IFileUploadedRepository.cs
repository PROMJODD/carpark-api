using Prom.LPR.Api.Models;

namespace Prom.LPR.Api.Database.Repositories
{
    public interface IFileUploadedRepository
    {
        public void SetCustomOrgId(string customOrgId);
        public MFileUploaded AddFileUploaded(MFileUploaded file);
    }
}
