using Microsoft.EntityFrameworkCore;
using Prom.LPR.Api.Models;

namespace Prom.LPR.Api.Database
{
    public interface IDataContext : IDisposable
    {
        int SaveChanges();
        public DbSet<MOrganization>? Organizations { get; set; }
        public DbSet<MApiKey>? ApiKeys { get; set; }
        public DbSet<MRole>? Roles { get; set; }
        public DbSet<MFileUploaded>? FileUploadeds { get; set; }
        public DbSet<MUser>? Users { get; set; }
        public DbSet<MOrganizationUser>? OrganizationUsers { get; set; }
    }
}