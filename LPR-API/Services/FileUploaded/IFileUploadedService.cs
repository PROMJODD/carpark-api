using Prom.LPR.Api.Models;

namespace Prom.LPR.Api.Services
{
    public interface IFileUploadedService
    {
        public MFileUploaded AddFileUploaded(string orgId, MFileUploaded file);
    }
}
