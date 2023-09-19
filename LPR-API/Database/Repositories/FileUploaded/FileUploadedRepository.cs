using LinqKit;
using Prom.LPR.Api.Models;
using Prom.LPR.Api.ViewsModels;

namespace Prom.LPR.Api.Database.Repositories
{
    public class FileUploadedRepository : BaseRepository, IFileUploadedRepository
    {
        public FileUploadedRepository(IDataContext ctx)
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

        private ExpressionStarter<MFileUploaded> FilesUploadedPredicate(VMFileUploadedQuery param)
        {
            var pd = PredicateBuilder.New<MFileUploaded>();

            pd = pd.And(p => p.OrgId!.Equals(orgId));

            if ((param.UploadedApi != "") && (param.UploadedApi != null))
            {
                pd = pd.And(p => p.UploadedApi!.Equals(param.UploadedApi));
            }

            if ((param.VehicleProvince != "") && (param.VehicleProvince != null))
            {
                pd = pd.And(p => p.VehicleProvince!.Equals(param.VehicleProvince));
            }

            if ((param.VehicleLicense != "") && (param.VehicleLicense != null))
            {
                pd = pd.And(p => p.VehicleLicense!.Equals(param.VehicleLicense));
            }

            return pd;
        }

        public int GetFilesUploadedCount(VMFileUploadedQuery param)
        {
            try
            {
                var predicate = FilesUploadedPredicate(param);
                var cnt = context!.FileUploadeds!.Where(predicate).Count();

                return cnt;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IEnumerable<MFileUploaded> GetFilesUploaded(VMFileUploadedQuery param)
        {
            try
            {
                var predicate = FilesUploadedPredicate(param);
                var arr = context!.FileUploadeds!.Where(predicate)
                    .Skip(param.Offset!)
                    .Take(param.Limit!)
                    .OrderByDescending(e => e.UploadedDate)
                    .ToList();

                return arr;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
