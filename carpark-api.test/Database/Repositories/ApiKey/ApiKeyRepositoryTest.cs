using Moq;
using Xunit;
using Prom.LPR.Api.Database;
using Prom.LPR.Api.Models;
using Prom.LPR.Api.Database.Repositories;

namespace Prom.LPR.Test.Database.Repositories;

public class ApiKeyRepositoryTest
{
    private void CreateKey(List<MApiKey> list, string key, string desc, string orgId)
    {
        var r = new MApiKey
        {
            ApiKey = key,
            KeyDescription = desc,
            OrgId = orgId,
        };

        list.Add(r);
    }

    [Theory]
    [InlineData("key1", "default")]
    [InlineData("key2", "global")]
    public void GetApiKeyTest(string apiKey, string orgId)
    {
        var keys = new List<MApiKey>();
        CreateKey(keys, "key1", "xxxxxx", orgId);
        CreateKey(keys, "key2", "xxxxxx", orgId);

        var ctxMock = new Mock<IDataContext>();
        ctxMock.Setup(x => x.ApiKeys).Returns(DbContextMock.GetQueryableMockDbSet(keys));

        var repo = new ApiKeyRepository(ctxMock.Object);
        repo.SetCustomOrgId(orgId);

        var t = repo.GetApiKey(apiKey);
        var value = t.Result.ApiKey;

        Assert.Equal(apiKey, value);
    }

    [Theory]
    [InlineData("key1", "default")]
    [InlineData("key2", "global")]
    [InlineData("key3", "NotMatchOrg")]
    public void GetApiKeyOrgNotMatch(string apiKey, string orgId)
    {
        var keys = new List<MApiKey>();
        CreateKey(keys, "key1", "xxxxxx", "NotMatchOrg");
        CreateKey(keys, "key2", "xxxxxx", "NotMatchOrg");

        var ctxMock = new Mock<IDataContext>();
        ctxMock.Setup(x => x.ApiKeys).Returns(DbContextMock.GetQueryableMockDbSet(keys));

        var repo = new ApiKeyRepository(ctxMock.Object);
        repo.SetCustomOrgId(orgId);

        var t = repo.GetApiKey(apiKey);

        Assert.Null(t.Result);
    }

    [Fact]
    public void GetApiKeyWithException()
    {
        var keys = new List<MApiKey>();
        CreateKey(keys, null!, "xxxxxx", null!);

        var ctxMock = new Mock<IDataContext>();
        ctxMock.Setup(x => x.ApiKeys).Returns(DbContextMock.GetQueryableMockDbSet(keys));

        var repo = new ApiKeyRepository(ctxMock.Object);
        repo.SetCustomOrgId("fake");

        Action act = () => repo.GetApiKey("fake-api-key");
        var ex = Record.Exception(act);
        Assert.NotNull(ex);
    }

    //#### AddApiKey ####

    [Theory]
    [InlineData("key1", "default")]
    [InlineData("key2", "global")]
    [InlineData("key3", "NotMatchOrg")]
    public void AddApiKeySuccess(string apiKey, string orgId)
    {
        var m = new MApiKey() { ApiKey = apiKey, KeyDescription = "", OrgId = "" };
        var keys = new List<MApiKey>();

        var ctxMock = new Mock<IDataContext>();
        ctxMock.Setup(x => x.ApiKeys).Returns(DbContextMock.GetQueryableMockDbSet(keys));

        var repo = new ApiKeyRepository(ctxMock.Object);
        repo.SetCustomOrgId(orgId);

        var r = repo.AddApiKey(m);

        Assert.Equal(orgId, r.OrgId); // Make sure we save OrgId from SetCustomOrgId()
        Assert.Single(keys);
    }

    [Fact]
    public void AddApiKeyWithException()
    {
        var keys = new List<MApiKey>();

        var ctxMock = new Mock<IDataContext>();
        ctxMock.Setup(x => x.ApiKeys).Returns(DbContextMock.GetQueryableMockDbSet(keys));

        var repo = new ApiKeyRepository(ctxMock.Object);
        repo.SetCustomOrgId("fake");

        Action act = () => repo.AddApiKey(null!);
        var ex = Record.Exception(act);
        Assert.NotNull(ex);
    }

    //#### GetApiKeys ####
    [Theory]
    [InlineData("key1", "default", 2)]
    [InlineData("key2", "global", 1)]
    [InlineData("key3", "NotMatchOrg", 1)]
    public void GetApiKeysSuccess(string apiKey, string orgId, int cnt)
    {
        var keys = new List<MApiKey>();
        CreateKey(keys, "abbbb1", "xxxxxx", "fake");
        CreateKey(keys, "abbbb2", "xxxxxx", "default");
        CreateKey(keys, apiKey, "xxxxxx", orgId);

        var ctxMock = new Mock<IDataContext>();
        ctxMock.Setup(x => x.ApiKeys).Returns(DbContextMock.GetQueryableMockDbSet(keys));

        var repo = new ApiKeyRepository(ctxMock.Object);
        repo.SetCustomOrgId(orgId);

        var list = repo.GetApiKeys();
        var keyCnt = list.Count();

        Assert.Equal(cnt, keyCnt);
    }

    [Fact]
    public void GetApiKeysWithException()
    {
        var keys = new List<MApiKey>();
        CreateKey(keys, "abbbb1", "xxxxxx", null!);

        var ctxMock = new Mock<IDataContext>();
        ctxMock.Setup(x => x.ApiKeys).Returns(DbContextMock.GetQueryableMockDbSet(keys));

        var repo = new ApiKeyRepository(ctxMock.Object);
        repo.SetCustomOrgId("fake");

        Action act = () => repo.GetApiKeys();
        var ex = Record.Exception(act);
        Assert.NotNull(ex);
    }

    //#### DeleteApiKeyById ####
    [Theory]
    [InlineData("default", 3)]
    [InlineData("global", 2)] //Delete success
    [InlineData("NotMatchOrg", 3)]
    public void DeleteApiKeyByIdNoException(string orgId, int cnt)
    {
        var keys = new List<MApiKey>();
        CreateKey(keys, "abbbb1", "xxxxxx", "fake");
        CreateKey(keys, "abbbb2", "xxxxxx", "default");
        CreateKey(keys, "key1", "xxxxxx", orgId);

        var rm = keys[2];

        var ctxMock = new Mock<IDataContext>();
        ctxMock.Setup(x => x.ApiKeys).Returns(DbContextMock.GetQueryableMockDbSet(keys));

        var repo = new ApiKeyRepository(ctxMock.Object);
        repo.SetCustomOrgId("global");

        repo.DeleteApiKeyById(rm.KeyId.ToString()!);
        var keyCnt = keys.Count();

        Assert.Equal(cnt, keyCnt);
    }

    [Fact]
    public void DeleteApiKeyByIdException()
    {
        var keys = new List<MApiKey>();

        var ctxMock = new Mock<IDataContext>();
        ctxMock.Setup(x => x.ApiKeys).Returns(DbContextMock.GetQueryableMockDbSet(keys));

        var repo = new ApiKeyRepository(ctxMock.Object);
        repo.SetCustomOrgId("fake");

        Action act = () => repo.DeleteApiKeyById("invalid-guid-format");
        var ex = Record.Exception(act);
        Assert.NotNull(ex);
    }
}
