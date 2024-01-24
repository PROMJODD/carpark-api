using Moq;
using Xunit;
using Prom.LPR.Api.Database;
using Prom.LPR.Api.Models;
using Prom.LPR.Api.Database.Repositories;
using Prom.LPR.Api.Services;
using Prom.LPR.Test.Database;
using Prom.LPR.Api.ViewsModels;
using Microsoft.Extensions.Configuration;
using Prom.LPR.Api.ExternalServices.ObjectStorage;
using Microsoft.AspNetCore.Http;
using Prom.LPR.Api.ExternalServices.Cache;
using Microsoft.VisualBasic;
using System.Collections;

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

    private IConfiguration GetCofigFromDictionary(Dictionary<string, string> setting)
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(setting!)
            .Build();

        return configuration;
    }

    [Theory]
    [InlineData("default")]
    [InlineData("global")]
    public void AddFilesUploadedTest(string orgId)
    {
        var setting = new Dictionary<string, string>
        {
            {"LPR:bucket", "unittesting"},
        };
        var configuration = GetCofigFromDictionary(setting);

        var files = new List<MFileUploaded>();
        var repo = CreateRepository(orgId, files);

        var m = new MFileUploaded() { UploadedApi = "VehicleImageUpload" };
        var svc = new FileUploadedService(repo, new GoogleCloudStorage(), configuration, new GcsSignerMocked(), null!, null!);
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
        var setting = new Dictionary<string, string>
        {
            {"LPR:bucket", "unittesting"},
        };
        var configuration = GetCofigFromDictionary(setting);

        var files = new List<MFileUploaded>();
        GenerateFilesUploaded(files, loopCnt, orgId);

        var repo = CreateRepository(orgId, files);

        var m = new VMFileUploadedQuery() { UploadedApi = "VehicleImageUpload" };
        var svc = new FileUploadedService(repo, new GoogleCloudStorage(), configuration, new GcsSignerMocked(), null!, null!);
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
        var setting = new Dictionary<string, string>
        {
            {"LPR:bucket", "unittesting"},
        };
        var configuration = GetCofigFromDictionary(setting);

        var files = new List<MFileUploaded>();
        GenerateFilesUploaded(files, loopCnt, orgId);

        var repo = CreateRepository(orgId, files);

        var m = new VMFileUploadedQuery() { UploadedApi = "VehicleImageUpload", Limit = limit, Offset = offset };
        var svc = new FileUploadedService(repo, new GoogleCloudStorage(), configuration, new GcsSignerMocked(), null!, null!);
        var list = svc.GetFilesUploaded(orgId, m);

        Assert.Equal(expectedCount, list.Count());
    }

    private IFileUploadedService GetFileUploadSvc(string orgId, IGcsSigner signer, ICache cache)
    {
        var setting = new Dictionary<string, string>
        {
            {"LPR:bucket", "unittesting"},
            {"LPR:lprPath", "/service"},
            {"LPR:lprBaseUrl", "https://localhost"},
            {"LPR:lprAuthKey", "thisisauthkey"}
        };
        var configuration = GetCofigFromDictionary(setting);
        var analyzer = new LPRAnalyzerMocked();

        var sc = new Mock<IGcsClient>();
        var dat = new Google.Apis.Storage.v1.Data.Object();
        sc.Setup(x => x.UploadObject(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Stream>())).Returns(dat);
        var gcs = new GoogleCloudStorage();
        gcs.SetStorageClient(sc.Object);

        var files = new List<MFileUploaded>();
        var repo = CreateRepository(orgId, files);
        var svc = new FileUploadedService(repo, gcs, configuration, signer, cache, analyzer);

        return svc;
    }

    [Theory]
    [InlineData("default")]
    public void UploadFileNotFoundTest(string orgId)
    {        
        var svc = GetFileUploadSvc(orgId, new GcsSignerMocked(), null!);

        var f = new MImageUploaded();
        var context = new DefaultHttpContext();
        var result = svc.UploadFile(orgId, f, context);

        Assert.NotNull(result);
        Assert.Equal("NOTFOUND", result.Status);
    }

    [Theory]
    [InlineData("default")]
    public void UploadFileTest(string orgId)
    {
        var localPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        File.Create(localPath).Dispose();

        var svc = GetFileUploadSvc(orgId, new GcsSignerMocked(), null!);

        var f = new MImageUploaded();
        using var stream = new MemoryStream(File.ReadAllBytes(localPath).ToArray());
        var formFile = new FormFile(stream, 0, stream.Length, "image", "abcde.jpg");
        f.Image = formFile;

        var context = new DefaultHttpContext();
        context.Items.Add("Temp-Identity-Type", "unit-testing");
        var result = svc.UploadFile(orgId, f, context);

        Assert.NotNull(result);
        Assert.NotNull(result.LprData);
        Assert.NotNull(result.LprData.Data);
        Assert.NotNull(result.StorageData);
        Assert.NotNull(result.StorageData.StoragePath);
        Assert.Equal("OK", result.Status);
        Assert.Equal("Bangkok", result.LprData.Data.Province);
        Assert.EndsWith("jpg", result.StorageData.StoragePath);

        File.Delete(localPath);
    }

    private void PopulateStoragePath(List<MFileUploaded> list)
    {
        foreach (var f in list)
        {
            f.StoragePath = f.VehicleLicense;
        }
    }

    [Theory]
    [InlineData("default", 5, "lic-1", "gs://bucket/lic-1.pjg")]
    [InlineData("default", 5, "lic-2", "gs://bucket/lic-2.pjg")]
    [InlineData("default", 5, "lic-3", "")]
    public void GetFilesUploadedWithCacheTest(string orgId, int loopCnt, string key, string expectedValue)
    {
        var setting = new Dictionary<string, string>
        {
            {"LPR:bucket", "unittesting"},
        };
        var configuration = GetCofigFromDictionary(setting);

        var files = new List<MFileUploaded>();
        GenerateFilesUploaded(files, loopCnt, orgId);
        PopulateStoragePath(files);

        var repo = CreateRepository(orgId, files);
        var cached = new CacheMocked();

        var m = new VMFileUploadedQuery() { UploadedApi = "VehicleImageUpload", Limit = loopCnt, Offset = 1 };
        var svc = new FileUploadedService(repo, new GoogleCloudStorage(), configuration, new GcsSignerMocked(), cached, null!);
        var list = svc.GetFilesUploaded(orgId, m);

        //Convert to Hashtable
        var coll = new Hashtable();
        foreach (var f in list)
        {
            coll.Add(f.StoragePath!, f.PresignedUrl);
        }

        var actualValue = coll[key];
        Assert.Equal(expectedValue, actualValue);
    }
}
