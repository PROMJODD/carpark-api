namespace Prom.LPR.Api.DBContext;

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

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<MOrganization>();
    }
}
