using Moq;
using Xunit;
using Prom.LPR.Api.Database;
using Prom.LPR.Api.Models;
using Prom.LPR.Api.Database.Repositories;
using Prom.LPR.Api.Services;
using Prom.LPR.Test.Database;
using Prom.LPR.Api.ViewsModels;

namespace Prom.LPR.Test.Api.Services;

public class FileUploadedServiceTest
{
    private IFileUploadedRepository CreateRepository(string orgId, List<MFileUploaded> lists)
    {
        var ctxMock = new Mock<IDataContext>();
        ctxMock.Setup(x => x.FileUploadeds).Returns(DbContextMock.GetQueryableMockDbSet(lists));
        var repo = new FileUploadedRepository(ctxMock.Object);
        repo.SetCustomOrgId(orgId);

        return repo;
    }

    private void CreateFileUploaded(List<MFileUploaded> list, string OrgId, string license, string color, string vclass, string brand, string province)
    {
        var r = new MFileUploaded
        {
            OrgId = OrgId,
            VehicleLicense = license,
            VehicleColor = color,
            VehicleClass = vclass,
            VehicleBrand = brand,
            VehicleProvince = province,
            UploadedApi = "VehicleImageUpload"
        };

        list.Add(r);
    }

    private void GenerateFilesUploaded(List<MFileUploaded> list, int cnt, string OrgId)
    {
        for (int i=1; i<=cnt; i++)
        {
            CreateFileUploaded(list, OrgId, $"lic-{i}", $"color-{i}", $"class-{i}", $"brand-{i}", $"province-{i}");
        }

        CreateFileUploaded(list, "xxswww", $"lic-{cnt}", $"color-{cnt}", $"class-{cnt}", $"brand-{cnt}", $"province-{cnt}");
    }

    [Theory]
    [InlineData("default")]
    [InlineData("global")]
    public void AddFilesUploadedTest(string orgId)
    {
        var files = new List<MFileUploaded>();
        var repo = CreateRepository(orgId, files);

        var m = new MFileUploaded() { UploadedApi = "VehicleImageUpload" };
        var svc = new FileUploadedService(repo);
        var result = svc.AddFileUploaded(orgId, m);

        Assert.NotNull(result);
        Assert.Equal(orgId, result.OrgId);
        Assert.Equal(m.UploadedApi, result.UploadedApi);
    }

    [Theory]
    [InlineData("default", 5)]
    [InlineData("global", 4)]
    public void GetFilesUploadedCountTest(string orgId, int loopCnt)
    {
        var files = new List<MFileUploaded>();
        GenerateFilesUploaded(files, loopCnt, orgId);

        var repo = CreateRepository(orgId, files);

        var m = new VMFileUploadedQuery() { UploadedApi = "VehicleImageUpload" };
        var svc = new FileUploadedService(repo);
        var result = svc.GetFilesUploadedCount(orgId, m);

        Assert.Equal(loopCnt, result);
    }

    [Theory]
    [InlineData("default", 5, 2, 1, 1)]
    [InlineData("globalx", 4, 2, 1, 1)]
    [InlineData("globalx", 4, 1, 4, 4)]
    [InlineData("globalx", 4, 1, 5, 4)]
    [InlineData("globalx", 4, 1, 0, 0)]
    [InlineData("globalx", 4, 0, 1, 1)]
    [InlineData("globalx", 4, -1, 5, 4)]
    public void GetFilesUploadedTest(string orgId, int loopCnt, int offset, int limit, int expectedCount)
    {
        var files = new List<MFileUploaded>();
        GenerateFilesUploaded(files, loopCnt, orgId);

        var repo = CreateRepository(orgId, files);

        var m = new VMFileUploadedQuery() { UploadedApi = "VehicleImageUpload", Limit = limit, Offset = offset };
        var svc = new FileUploadedService(repo);
        var list = svc.GetFilesUploaded(orgId, m);

        Assert.Equal(expectedCount, list.Count());
    }
}
