using Serilog;
using Prom.LPR.Api.Database;
using Microsoft.EntityFrameworkCore;
using Prom.LPR.Api.Services;
using Prom.LPR.Api.Repositories;

namespace Prom.LPR.Worker
{
    class Program
    {
        public static void Main(string[] args)
        {
            var log = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();
            Log.Logger = log;

            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddControllers()
                .AddJsonOptions(options => options.JsonSerializerOptions.WriteIndented = true);

            var cfg = builder.Configuration;

            var connStr = $"Host={cfg["PostgreSQL:Host"]}; Database={cfg["PostgreSQL:Database"]}; Username={cfg["PostgreSQL:User"]}; Password={cfg["PostgreSQL:Password"]}";

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDbContext<DataContext>(options => options.UseNpgsql(connStr));
            builder.Services.AddTransient<DataSeeder>();
            builder.Services.AddScoped<IOrganizationService, OrganizationService>();
            builder.Services.AddScoped<IOrganizationRepository, OrganizationRepository>();
            builder.Services.AddScoped<IApiKeyService, ApiKeyService>();
            builder.Services.AddScoped<IApiKeyRepository, ApiKeyRepository>();

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();
                dbContext.Database.Migrate();

                var service = scope.ServiceProvider.GetRequiredService<DataSeeder>();
                service.Seed();
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            //app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}
