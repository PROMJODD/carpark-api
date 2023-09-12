using Prom.LPR.Api.Models;

namespace Prom.LPR.Api.Database.Repositories
{
    public class FileUploadedRepository : BaseRepository, IFileUploadedRepository
    {
        public FileUploadedRepository(DataContext ctx)
        {
            context = ctx;
        }

        public MFileUploaded AddFileUploaded(MFileUploaded file)
        {
            try
            {
                file.OrgId = orgId;

                context!.FileUploadeds!.AddAsync(file);
                context.SaveChanges();
            }
            catch (Exception)
            {
                throw;
            }

            return file;
        }
    }
}
