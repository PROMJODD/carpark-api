using Moq;
using Xunit;
using Prom.LPR.Api.Database;
using Prom.LPR.Api.Models;
using Prom.LPR.Api.Database.Repositories;
using Prom.LPR.Api.Services;
using Prom.LPR.Test.Database;

namespace Prom.LPR.Test.Api.Services;

public class ApiKeyServiceTest
{
    private IApiKeyRepository CreateRepository(string orgId, List<MApiKey> lists)
    {
        var ctxMock = new Mock<IDataContext>();
        ctxMock.Setup(x => x.ApiKeys).Returns(DbContextMock.GetQueryableMockDbSet(lists));
        var repo = new ApiKeyRepository(ctxMock.Object);
        repo.SetCustomOrgId(orgId);

        return repo;
    }

    private void CreateApiKey(List<MApiKey> list, string OrgId, string key, string desc, DateTime? expireDate)
    {
        var r = new MApiKey
        {
            OrgId = OrgId,
            ApiKey = key,
            KeyDescription = desc,
            KeyExpiredDate = expireDate
        };

        list.Add(r);
    }

    private void GenerateApiKeys(List<MApiKey> list, int cnt, string OrgId, DateTime? expireDate)
    {
        for (int i=1; i<=cnt; i++)
        {
            CreateApiKey(list, OrgId, $"key-{i}", $"desc-{i}", expireDate);
        }

        CreateApiKey(list, "xxswww", $"key-{cnt}", $"desc-{cnt}", expireDate);
    }

    [Theory]
    [InlineData("default", 5, 5)]
    [InlineData("globalx", 4, 4)]
    [InlineData("default", 4, 4)]
    public void GetApiKeysTest(string orgId, int loopCnt, int expectedCount)
    {
        var keys = new List<MApiKey>();
        GenerateApiKeys(keys, loopCnt, orgId, DateTime.Now);

        var repo = CreateRepository(orgId, keys);

        var svc = new ApiKeyService(repo);
        var list = svc.GetApiKeys(orgId);

        Assert.Equal(expectedCount, list.Count());
    }

    [Theory]
    [InlineData("default", 5, "key-1")]
    [InlineData("globalx", 4, "key-2")]
    [InlineData("default", 4, "key-3")]
    [InlineData("default", 4, "key-4")]
    public void GetApiKeyFound(string orgId, int loopCnt, string apiKey)
    {
        var keys = new List<MApiKey>();
        GenerateApiKeys(keys, loopCnt, orgId, DateTime.Now);

        var repo = CreateRepository(orgId, keys);

        var svc = new ApiKeyService(repo);
        var t = svc.GetApiKey(orgId, apiKey);
        var key = t.Result;

        Assert.NotNull(key);
        Assert.Equal(orgId, key.OrgId);
        Assert.Equal(apiKey, key.ApiKey);
    }

    [Theory]
    [InlineData("default", 5, "key-0")]
    [InlineData("globalx", 4, "key-0")]
    [InlineData("default", 4, "key-0")]
    [InlineData("xxxxxxx", 4, "key-0")]
    public void GetApiKeyNotFound(string orgId, int loopCnt, string apiKey)
    {
        var keys = new List<MApiKey>();
        GenerateApiKeys(keys, loopCnt, orgId, DateTime.Now);

        var repo = CreateRepository(orgId, keys);

        var svc = new ApiKeyService(repo);
        var t = svc.GetApiKey(orgId, apiKey);
        var key = t.Result;

        Assert.Null(key);
    }

    [Theory]
    [InlineData("default", 5, "key-0")]
    [InlineData("globalx", 4, "key-0")]
    [InlineData("default", 4, "key-0")]
    [InlineData("xxxxxxx", 4, "key-0")]
    public void VerifyApiKeyNotFound(string orgId, int loopCnt, string apiKey)
    {
        var keys = new List<MApiKey>();
        GenerateApiKeys(keys, loopCnt, orgId, DateTime.Now);

        var repo = CreateRepository(orgId, keys);

        var svc = new ApiKeyService(repo);
        var key = svc.VerifyApiKey(orgId, apiKey);

        Assert.NotNull(key);
        Assert.Equal("NOTFOUND", key.Status);
    }

    [Theory]
    [InlineData("default", 5, "key-1")]
    [InlineData("globalx", 4, "key-1")]
    [InlineData("default", 4, "key-1")]
    [InlineData("xxxxxxx", 4, "key-1")]
    public void VerifyApiKeyExpire(string orgId, int loopCnt, string apiKey)
    {
        var keys = new List<MApiKey>();
        GenerateApiKeys(keys, loopCnt, orgId, DateTime.Now.AddHours(-1));

        var repo = CreateRepository(orgId, keys);

        var svc = new ApiKeyService(repo);
        var key = svc.VerifyApiKey(orgId, apiKey);

        Assert.NotNull(key);
        Assert.Equal("EXPIRED", key.Status);
    }

    [Theory]
    [InlineData("default", 5, "key-1")]
    [InlineData("globalx", 4, "key-1")]
    [InlineData("default", 4, "key-1")]
    [InlineData("xxxxxxx", 4, "key-1")]
    public void VerifyApiKeyNotExpireIfNull(string orgId, int loopCnt, string apiKey)
    {
        var keys = new List<MApiKey>();
        GenerateApiKeys(keys, loopCnt, orgId, null);

        var repo = CreateRepository(orgId, keys);

        var svc = new ApiKeyService(repo);
        var key = svc.VerifyApiKey(orgId, apiKey);

        Assert.NotNull(key);
        Assert.Equal("OK", key.Status);
    }

    [Theory]
    [InlineData("default", 5, "key-1")]
    [InlineData("globalx", 4, "key-1")]
    [InlineData("default", 4, "key-1")]
    [InlineData("xxxxxxx", 4, "key-1")]
    public void VerifyApiKeyNotExpireIfNotNull1(string orgId, int loopCnt, string apiKey)
    {
        var keys = new List<MApiKey>();
        GenerateApiKeys(keys, loopCnt, orgId, DateTime.Now.AddDays(1));

        var repo = CreateRepository(orgId, keys);

        var svc = new ApiKeyService(repo);
        var key = svc.VerifyApiKey(orgId, apiKey);

        Assert.NotNull(key);
        Assert.Equal("OK", key.Status);
    }

    [Theory]
    [InlineData("default", 5, "key-1")]
    [InlineData("globalx", 4, "key-1")]
    [InlineData("default", 4, "key-1")]
    [InlineData("xxxxxxx", 4, "key-1")]
    public void VerifyApiKeyNotExpireIfNotNull2(string orgId, int loopCnt, string apiKey)
    {
        var dtm = DateTime.Now;

        var keys = new List<MApiKey>();
        GenerateApiKeys(keys, loopCnt, orgId, dtm);

        var repo = CreateRepository(orgId, keys);

        var svc = new ApiKeyService(repo);
        svc.SetCompareDate(dtm);
        var key = svc.VerifyApiKey(orgId, apiKey);

        Assert.NotNull(key);
        Assert.Equal("OK", key.Status);
    }

    [Theory]
    [InlineData("default", 5)]
    [InlineData("globalx", 4)]
    [InlineData("default", 4)]
    [InlineData("xxxxxxx", 4)]
    public void AddApiKeyNoDuplicate(string orgId, int loopCnt)
    {
        var keys = new List<MApiKey>();
        GenerateApiKeys(keys, loopCnt, orgId, null);

        var repo = CreateRepository(orgId, keys);
        var apiKey = new MApiKey() { ApiKey = "fake-api-key" };

        var svc = new ApiKeyService(repo);
        var key = svc.AddApiKey(orgId, apiKey);

        Assert.NotNull(key);
        Assert.NotNull(key.ApiKey);
        Assert.Equal("OK", key.Status);
        Assert.Equal(orgId, key.ApiKey.OrgId);
    }

    [Theory]
    [InlineData("default", 5)]
    [InlineData("globalx", 4)]
    [InlineData("default", 4)]
    [InlineData("xxxxxxx", 4)]
    public void AddApiKeyDuplicate(string orgId, int loopCnt)
    {
        var keys = new List<MApiKey>();
        GenerateApiKeys(keys, loopCnt, orgId, null);

        var repo = CreateRepository(orgId, keys);
        var apiKey = new MApiKey() { ApiKey = "key-1" };

        var svc = new ApiKeyService(repo);
        var key = svc.AddApiKey(orgId, apiKey);

        Assert.NotNull(key);
        Assert.Null(key.ApiKey);
        Assert.Equal("DUPLICATE", key.Status);
    }

    [Theory]
    [InlineData("default", 5)]
    [InlineData("globalx", 4)]
    public void DeleteApiKeyFound(string orgId, int loopCnt)
    {
        var keys = new List<MApiKey>();
        GenerateApiKeys(keys, loopCnt, orgId, null);
        var id = keys[0].KeyId;

        var repo = CreateRepository(orgId, keys);

        var svc = new ApiKeyService(repo);
        var key = svc.DeleteApiKeyById(orgId, id.ToString()!);

        Assert.NotNull(key);
        Assert.Equal("OK", key.Status);
        Assert.Equal(loopCnt, keys.Count); //not loopCnt -1 because there is an extra API Key in the list
    }

    [Theory]
    [InlineData("default", 5)]
    [InlineData("globalx", 4)]
    public void DeleteApiKeyNotFound(string orgId, int loopCnt)
    {
        var keys = new List<MApiKey>();
        GenerateApiKeys(keys, loopCnt, orgId, null);
        var id = Guid.NewGuid();

        var repo = CreateRepository(orgId, keys);

        var svc = new ApiKeyService(repo);
        var key = svc.DeleteApiKeyById(orgId, id.ToString()!);

        Assert.NotNull(key);
        Assert.Equal("NOTFOUND", key.Status);
    }

    [Theory]
    [InlineData("default", 5)]
    [InlineData("globalx", 4)]
    public void DeleteApiKeyInvalid(string orgId, int loopCnt)
    {
        var keys = new List<MApiKey>();
        GenerateApiKeys(keys, loopCnt, orgId, null);
        var id = "fake-uuid";

        var repo = CreateRepository(orgId, keys);

        var svc = new ApiKeyService(repo);
        var key = svc.DeleteApiKeyById(orgId, id);

        Assert.NotNull(key);
        Assert.Equal("UUID_INVALID", key.Status);
    }
}
