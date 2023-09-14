using Prom.LPR.Api.Models;
using Prom.LPR.Api.ViewsModels;

namespace Prom.LPR.Api.Services
{
    public interface IFileUploadedService
    {
        public MFileUploaded AddFileUploaded(string orgId, MFileUploaded file);
        public IEnumerable<MFileUploaded> GetFilesUploaded(string orgId, VMFileUploadedQuery param);
        public int GetFilesUploadedCount(string orgId, VMFileUploadedQuery param);
    }
}
