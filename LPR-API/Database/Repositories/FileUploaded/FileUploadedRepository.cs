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

        public IEnumerable<MFileUploaded> GetFilesUploaded()
        {
            try
            {
                var arr = context!.FileUploadeds!.Where(x => x.OrgId!.Equals(orgId)).ToList();
                return arr;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
