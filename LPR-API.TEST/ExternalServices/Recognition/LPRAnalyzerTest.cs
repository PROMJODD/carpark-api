using Xunit;
using Prom.LPR.Api.ExternalServices.Recognition;
using Microsoft.Extensions.Configuration;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace Prom.LPR.Test.Api.ExternalServices.Recognition;

public class LPRAnalyzerTest
{
    private IConfiguration GetCofigFromDictionary(Dictionary<string, string> setting)
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(setting!)
            .Build();

        return configuration;
    }

    [Fact]
    public void LPRAnalyzerValueTest()
    {
        var setting = new Dictionary<string, string>
        {
            {"LPR:lprBaseUrl", "https://thisis-base-url"},
            {"LPR:lprPath", "/service"},
            {"LPR:lprAuthKey", "thisisauthkey"}
        };

        var configuration = GetCofigFromDictionary(setting);
        var m = new LPRAnalyzer(configuration);

        var uploadKey = m.GetFormUploadKey();
        var http = m.GetHttpClient();
        var req = m.GetRequestMessage();

        Assert.NotNull(http);
        Assert.NotNull(req);

        Assert.Equal("image", uploadKey);
        Assert.Equal(HttpMethod.Post, req.Method);
    }
}
