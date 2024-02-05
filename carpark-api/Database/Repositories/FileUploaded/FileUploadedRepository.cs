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
                pd = pd.And(p => p.VehicleProvince!.Contains(param.VehicleProvince));
            }

            if ((param.VehicleLicense != "") && (param.VehicleLicense != null))
            {
                pd = pd.And(p => p.VehicleLicense!.Contains(param.VehicleLicense));
            }

            if ((param.FullTextSearch != "") && (param.FullTextSearch != null))
            {
                var fullTextPd = PredicateBuilder.New<MFileUploaded>();
                fullTextPd = fullTextPd.Or(p => p.VehicleProvince!.Contains(param.FullTextSearch));
                fullTextPd = fullTextPd.Or(p => p.VehicleLicense!.Contains(param.FullTextSearch));
                fullTextPd = fullTextPd.Or(p => p.VehicleBrand!.Contains(param.FullTextSearch));
                fullTextPd = fullTextPd.Or(p => p.VehicleColor!.Contains(param.FullTextSearch));
                fullTextPd = fullTextPd.Or(p => p.VehicleClass!.Contains(param.FullTextSearch));

                pd = pd.And(fullTextPd);
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
            var limit = 0;
            var offset = 0;

            //Param will never be null

            if (param.Offset > 0)
            {
                //Convert to zero base
                offset = param.Offset-1;
            }

            if (param.Limit > 0)
            {
                limit = param.Limit;
            }

            var predicate = FilesUploadedPredicate(param!);
            var arr = context!.FileUploadeds!.Where(predicate)
                .OrderByDescending(e => e.UploadedDate)
                .Skip(offset)
                .Take(limit)
                .ToList();

            return arr;
        }
    }
}
