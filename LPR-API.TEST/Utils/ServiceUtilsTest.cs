using Xunit;
using Moq;
using Prom.LPR.Api.Utils;
using Microsoft.AspNetCore.Http;

namespace Prom.LPR.Test.Api.Utils;

public class ServiceUtilsTest
{
    [Theory]
    [InlineData("0991e9ac-9ff1-4ec7-ab9a-4ae3aa292f59", true)]
    [InlineData("abcde-xxxxx-kkkkk", false)]
    public void CheckIfGuiIdValid(string id, bool expectedValue)
    {
        var value = ServiceUtils.IsGuidValid(id);
        Assert.Equal(expectedValue, value);
    }

    [Theory]
    [InlineData("/api/ApiGroup/org/00001/action/GetSomething", "00001")]
    [InlineData("/api/ApiGroup/org/a.b.c/action/GetSomething", "a.b.c")]
    [InlineData("/api/ApiGroup/org/a-b-c/action/GetSomething", "a-b-c")]
    public void CheckIfOrgIdIsCorrect(string path, string expectedValue)
    {
        var req = new Mock<HttpRequest>();
        req.Setup(x => x.Path).Returns(path);

        var value = ServiceUtils.GetOrgId(req.Object);

        Assert.Equal(expectedValue, value);
    }
}
