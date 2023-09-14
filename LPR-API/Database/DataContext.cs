namespace Prom.LPR.Api.Database;

using Prom.LPR.Api.Models;
using Microsoft.EntityFrameworkCore;

public class DataContext : DbContext
{
    protected readonly IConfiguration Configuration;

    public DataContext(IConfiguration configuration, DbContextOptions<DataContext> options) : base(options)
    {
        Configuration = configuration;
    }

    public DbSet<MOrganization>? Organizations { get; set; }
    public DbSet<MApiKey>? ApiKeys { get; set; }
    public DbSet<MRole>? Roles { get; set; }
    public DbSet<MFileUploaded>? FileUploadeds { get; set; }
    public DbSet<MUser>? Users { get; set; }
    public DbSet<MOrganizationUser>? OrganizationUsers { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<MOrganization>();
        builder.Entity<MApiKey>();
        builder.Entity<MRole>();
        builder.Entity<MFileUploaded>();
        builder.Entity<MUser>();
        builder.Entity<MOrganizationUser>();
    }
}
