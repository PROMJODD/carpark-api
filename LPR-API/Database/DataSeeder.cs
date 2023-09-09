namespace Prom.LPR.Api.Database;

using Prom.LPR.Api.Models;

public class DataSeeder
{
    private readonly DataContext context;

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

    public void Seed()
    {
        SeedDefaultOrganization();
        UpdateDefaultOrganizationCustomId();
    }
}