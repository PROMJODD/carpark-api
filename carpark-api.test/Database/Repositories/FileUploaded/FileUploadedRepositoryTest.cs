using Moq;
using Xunit;
using Prom.LPR.Api.Database;
using Prom.LPR.Api.Models;
using Prom.LPR.Api.Database.Repositories;
using Prom.LPR.Api.ViewsModels;
using System.Reflection;

namespace Prom.LPR.Test.Database.Repositories;

public class FileUploadedRepositoryTest
{
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

    private void GenerateFilesUploadedError(List<MFileUploaded> list, int cnt, string OrgId)
    {
        for (int i=1; i<=cnt; i++)
        {
            CreateFileUploaded(list, OrgId, null!, null!, null!, null!, null!);
        }

        CreateFileUploaded(list, "xxswww", $"lic-{cnt}", $"color-{cnt}", $"class-{cnt}", $"brand-{cnt}", $"province-{cnt}");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    public void GetFilesUploadedCountTest(int cnt)
    {
        var param = new VMFileUploadedQuery();
        string orgId = "fake";

        var files = new List<MFileUploaded>();
        GenerateFilesUploaded(files, cnt, orgId);

        var ctxMock = new Mock<IDataContext>();
        ctxMock.Setup(x => x.FileUploadeds).Returns(DbContextMock.GetQueryableMockDbSet(files));

        var repo = new FileUploadedRepository(ctxMock.Object);
        repo.SetCustomOrgId(orgId);

        var c = repo.GetFilesUploadedCount(param);

        Assert.Equal(cnt, c);
    }

    [Theory]
    [InlineData(1)]
    public void GetFilesUploadedCountException(int cnt)
    {
        var param = new VMFileUploadedQuery() { VehicleLicense = "lic-0xxx1"};
        string orgId = "fake";

        var files = new List<MFileUploaded>();
        GenerateFilesUploadedError(files, cnt, orgId);

        var ctxMock = new Mock<IDataContext>();
        ctxMock.Setup(x => x.FileUploadeds).Returns(DbContextMock.GetQueryableMockDbSet(files));

        var repo = new FileUploadedRepository(ctxMock.Object);
        repo.SetCustomOrgId(orgId);

        Action act = () => repo.GetFilesUploadedCount(param);
        var ex = Record.Exception(act);
        Assert.NotNull(ex);
    }

    [Theory]
    [InlineData("VehicleLicense", "lic-1", 5, 1)]
    [InlineData("VehicleProvince", "province-3", 5, 1)]
    [InlineData("UploadedApi", "VehicleImageUpload", 5, 5)]
    public void GetFilesUploadedCountWithFilterNull(string fieldName, string fieldValue, int cnt, int expectedCnt)
    {
        var param = new VMFileUploadedQuery() { VehicleLicense = null, VehicleProvince = null, UploadedApi = null };

        Type myType = typeof(VMFileUploadedQuery);
        var myFieldInfo = myType.GetProperty(fieldName, BindingFlags.Public | BindingFlags.Instance);
        myFieldInfo!.SetValue(param, fieldValue);

        string orgId = "fake";

        var files = new List<MFileUploaded>();
        GenerateFilesUploaded(files, cnt, orgId);

        var ctxMock = new Mock<IDataContext>();
        ctxMock.Setup(x => x.FileUploadeds).Returns(DbContextMock.GetQueryableMockDbSet(files));

        var repo = new FileUploadedRepository(ctxMock.Object);
        repo.SetCustomOrgId(orgId);

        var c = repo.GetFilesUploadedCount(param);

        Assert.Equal(expectedCnt, c);
    }

    [Theory]
    [InlineData("VehicleLicense", "lic-1", 5, 1)]
    [InlineData("VehicleProvince", "province-3", 5, 1)]
    [InlineData("UploadedApi", "VehicleImageUpload", 5, 5)]
    public void GetFilesUploadedCountWithFilterEmpty(string fieldName, string fieldValue, int cnt, int expectedCnt)
    {
        var param = new VMFileUploadedQuery() { VehicleLicense = "", VehicleProvince = "", UploadedApi = "" };

        Type myType = typeof(VMFileUploadedQuery);
        var myFieldInfo = myType.GetProperty(fieldName, BindingFlags.Public | BindingFlags.Instance);
        myFieldInfo!.SetValue(param, fieldValue);

        string orgId = "fake";

        var files = new List<MFileUploaded>();
        GenerateFilesUploaded(files, cnt, orgId);

        var ctxMock = new Mock<IDataContext>();
        ctxMock.Setup(x => x.FileUploadeds).Returns(DbContextMock.GetQueryableMockDbSet(files));

        var repo = new FileUploadedRepository(ctxMock.Object);
        repo.SetCustomOrgId(orgId);

        var c = repo.GetFilesUploadedCount(param);

        Assert.Equal(expectedCnt, c);
    }

    [Theory]
    [InlineData("lic-1", 5)]
    [InlineData("lic-2", 5)]
    public void AddFileUploadedSuccess(string lic, int loopCnt)
    {
        var files = new List<MFileUploaded>();
        GenerateFilesUploaded(files, loopCnt, "fake-org-id");

        var ctxMock = new Mock<IDataContext>();
        ctxMock.Setup(x => x.FileUploadeds).Returns(DbContextMock.GetQueryableMockDbSet(files));

        var repo = new FileUploadedRepository(ctxMock.Object);

        var u = new MFileUploaded() { VehicleLicense = lic, OrgId = "fake-org-id" };
        var retUser = repo.AddFileUploaded(u);
        var cnt = files.Count();

        Assert.Equal(loopCnt+2, cnt);
        Assert.Equal(u.VehicleLicense, lic);
    }

    [Theory]
    [InlineData(5)]
    [InlineData(1)]
    public void AddFileUploadedWithException(int loopCnt)
    {
        var files = new List<MFileUploaded>();
        GenerateFilesUploaded(files, loopCnt, "fake-org-id");

        var ctxMock = new Mock<IDataContext>();
        ctxMock.Setup(x => x.FileUploadeds).Returns(DbContextMock.GetQueryableMockDbSet(files));

        var repo = new FileUploadedRepository(ctxMock.Object);

        Action act = () => repo.AddFileUploaded(null!);
        var ex = Record.Exception(act);
        Assert.NotNull(ex);
    }

    [Theory]
    [InlineData(100, 1, 5)]
    [InlineData(100, 1, 1)]
    [InlineData(100, 3, 2)]
    public void GetFilesUploadedSuccess(int loopCnt, int offset, int limit)
    {
        var orgId = "fake-org-id";

        var files = new List<MFileUploaded>();
        GenerateFilesUploaded(files, loopCnt, orgId);

        var ctxMock = new Mock<IDataContext>();
        ctxMock.Setup(x => x.FileUploadeds).Returns(DbContextMock.GetQueryableMockDbSet(files));

        var repo = new FileUploadedRepository(ctxMock.Object);
        repo.SetCustomOrgId(orgId);

        var u = new VMFileUploadedQuery() { Offset = offset, Limit = limit };
        var list = repo.GetFilesUploaded(u);
        var cnt = list.Count();

        Assert.Equal(limit, cnt);
    }


    [Theory]
    [InlineData(1)]
    public void GetFilesUploadedException(int cnt)
    {
        var param = new VMFileUploadedQuery() { VehicleLicense = "lic-0xxx1"};
        string orgId = "fake";

        var files = new List<MFileUploaded>();
        GenerateFilesUploadedError(files, cnt, orgId);

        var ctxMock = new Mock<IDataContext>();
        ctxMock.Setup(x => x.FileUploadeds).Returns(DbContextMock.GetQueryableMockDbSet(files));

        var repo = new FileUploadedRepository(ctxMock.Object);
        repo.SetCustomOrgId(orgId);

        Action act = () => repo.GetFilesUploaded(param);
        var ex = Record.Exception(act);
        Assert.NotNull(ex);
    }
}
