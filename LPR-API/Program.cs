using Serilog;
using Prom.LPR.Api.Database;
using Microsoft.EntityFrameworkCore;
using Prom.LPR.Api.Services;
using Prom.LPR.Api.Database.Repositories;
using Prom.LPR.Api.Authentications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Prom.LPR.Api.Database.Seeders;
using Prom.LPR.Api.Authorizations;
using System.Diagnostics.CodeAnalysis;
using Prom.LPR.Api.ExternalServices.ObjectStorage;
using Prom.LPR.Api.ExternalServices.Recognition;
using Microsoft.OpenApi.Models;

namespace Prom.LPR.Worker
{
    [ExcludeFromCodeCoverage]
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

            builder.Services.AddScoped<IDataContext, DataContext>();
            builder.Services.AddScoped<IApiKeyService, ApiKeyService>();
            builder.Services.AddScoped<IOrganizationService, OrganizationService>();
            builder.Services.AddScoped<IRoleService, RoleService>();
            builder.Services.AddScoped<IFileUploadedService, FileUploadedService>();
            builder.Services.AddScoped<IUserService, UserService>();

            builder.Services.AddScoped<IOrganizationRepository, OrganizationRepository>();
            builder.Services.AddScoped<IApiKeyRepository, ApiKeyRepository>();
            builder.Services.AddScoped<IRoleRepository, RoleRepository>();
            builder.Services.AddScoped<IFileUploadedRepository, FileUploadedRepository>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();

            builder.Services.AddTransient<IAuthorizationHandler, GenericRbacHandler>();
            builder.Services.AddScoped<IBasicAuthenticationRepo, BasicAuthenticationRepo>();
            builder.Services.AddScoped<IBearerAuthenticationRepo, BearerAuthenticationRepo>();
            builder.Services.AddScoped<IObjectStorage, GoogleCloudStorage>();
            builder.Services.AddScoped<IImageAnalyzer, LPRAnalyzer>();

            builder.Services.AddAuthentication("BasicOrBearer")
                .AddScheme<AuthenticationSchemeOptions, AuthenticationHandlerProxy>("BasicOrBearer", null);

            builder.Services.AddAuthorization(options => {
                var defaultAuthorizationPolicyBuilder = new AuthorizationPolicyBuilder("BasicOrBearer");
                defaultAuthorizationPolicyBuilder = defaultAuthorizationPolicyBuilder.RequireAuthenticatedUser();
                options.DefaultPolicy = defaultAuthorizationPolicyBuilder.Build();

                options.AddPolicy("GenericRolePolicy", policy => policy.AddRequirements(new GenericRbacRequirement()));
            });

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
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}
