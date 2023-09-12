namespace Prom.LPR.Api.Database.Seeders;

using PasswordGenerator;
using Prom.LPR.Api.Models;

public class DataSeeder
{
    private readonly DataContext context;
    private Password pwd = new Password(32);

    public DataSeeder(DataContext ctx)
    {
        context = ctx;
    }

    private void SeedDefaultOrganization()
    {
        if (context == null)
        {
            return;
        }

        if (context.Organizations == null)
        {
            return;
        }

        if (!context.Organizations.Any())
        {
            var orgs = new List<MOrganization>()
            {
                new MOrganization
                {
                    OrgId = Guid.NewGuid(),
                    OrgName = "DEFAULT",
                    OrgDescription = "Default initial created organization",
                    OrgCreatedDate = DateTime.UtcNow,
                    OrgCustomId = "default"
                }
            };

            context.Organizations.AddRange(orgs);
            context.SaveChanges();
        }
    }

    private void SeedGlobalOrganization()
    {
        if (context == null)
        {
            return;
        }

        if (context.Organizations == null)
        {
            return;
        }

        string orgId = "global";

        var query = context.Organizations!.Where(x => x.OrgCustomId!.Equals(orgId)).FirstOrDefault();
        if (query == null)
        {
            //Create if not exist
            var orgs = new List<MOrganization>()
            {
                new MOrganization
                {
                    OrgId = Guid.NewGuid(),
                    OrgName = "GLOBAL",
                    OrgDescription = "Global/Root initial created organization",
                    OrgCreatedDate = DateTime.UtcNow,
                    OrgCustomId = orgId
                }
            };
            context.Organizations.AddRange(orgs);

            var apiKey = new MApiKey()
            {
                KeyId = Guid.NewGuid(),
                KeyCreatedDate = DateTime.UtcNow,
                OrgId = orgId,
                ApiKey = pwd.Next(),
                KeyDescription = "Auto created root key"
            };
            context.ApiKeys!.Add(apiKey);

            context.SaveChanges();
        }
    }

    private void UpdateDefaultOrganizationCustomId()
    {
        if (context == null)
        {
            return;
        }

        if (context.Organizations == null)
        {
            return;
        }

        try
        {
            var query = context.Organizations!.Where(x => x.OrgName!.Equals("DEFAULT")).FirstOrDefault();
            if (query == null)
            {
                throw new Exception("Default organization 'DEFAULT' not found!!!");
            }
            query.OrgCustomId = "default";
            context.SaveChanges();
        }
        catch (Exception)
        {
            throw;
        }
    }

    private void AddRole(string name, string definition, string level, string desc)
    {
        var query = context.Roles!.Where(x => x.RoleName!.Equals(name)).FirstOrDefault();
        if (query != null)
        {
            //Already exist
            return;
        }

        var r = new MRole() 
        {
            RoleName = name,
            RoleDefinition = definition,
            RoleLevel = level,
            RoleDescription = desc
        };

        context!.Roles!.Add(r);
    }

    private void SeedDefaultRoles()
    {
        AddRole("CREATOR", "Admin:CreateOrganization,ApiKey:AddApiKey", "ADMIN", "Organization creator");
        AddRole("OWNER", ".+:.+", "ORGANIZATION", "Organization Owner");
        AddRole("VIEWER", ".+:Get.+", "ORGANIZATION", "Organization Viewer");
        AddRole("EDITOR", ".+:Add.+,.+:Update.+,.+:Delete.+", "ORGANIZATION", "Organization Editor");
        AddRole("UPLOADER", "FileUpload:Upload.+Image", "ORGANIZATION", "Organization File Uploader");

        context.SaveChanges();
    }

    private void UpdateApiKeyRole()
    {
        var apiKeys = context.ApiKeys!.Where(x => x.RolesList!.Equals(null) || x.RolesList!.Equals("")).ToList();
        apiKeys.ForEach(a => a.RolesList = "OWNER");
        context.SaveChanges();
    }

    public void Seed()
    {
        SeedDefaultOrganization();
        UpdateDefaultOrganizationCustomId();

        SeedGlobalOrganization();
        SeedDefaultRoles();
        UpdateApiKeyRole();
    }
}