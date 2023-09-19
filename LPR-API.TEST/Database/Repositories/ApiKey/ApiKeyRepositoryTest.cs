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
}
