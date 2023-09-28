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
            file.OrgId = orgId;

            context!.FileUploadeds!.Add(file);
            context.SaveChanges();

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
            var predicate = FilesUploadedPredicate(param);
            var cnt = context!.FileUploadeds!.Where(predicate).Count();

            return cnt;
        }

        public IEnumerable<MFileUploaded> GetFilesUploaded(VMFileUploadedQuery param)
        {
            var predicate = FilesUploadedPredicate(param);
            var arr = context!.FileUploadeds!.Where(predicate)
                .Skip(param.Offset! - 1)
                .Take(param.Limit!)
                .OrderByDescending(e => e.UploadedDate)
                .ToList();

            return arr;
        }
    }
}
