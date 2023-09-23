using Microsoft.Extensions.Configuration;
using Prom.LPR.Api.ExternalServices.Recognition;
using Xunit;

namespace Prom.LPR.Test.Api.ExternalServices.Recognition;

public class ImageAnalyzerHttpBaseTest
{
    private IConfiguration GetCofigFromDictionary(Dictionary<string, string> setting)
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(setting!)
            .Build();

        return configuration;
    }

    [Fact]
    public void ImageAnalyzerHttpBaseStringTest()
    {
        var path = "/tmp/test.tmp";
        File.Create(path).Dispose();

        var m = new LPRAnalyzerMocked();
        var json = m.AnalyzeFile(path);

        File.Delete(path);

        Assert.Equal("{\"field\":\"value\"}", json);
    }

    [Fact]
    public void ImageAnalyzerHttpBaseObjTest()
    {
        var path = "/tmp/test.tmp";
        File.Create(path).Dispose();

        var m = new LPRAnalyzerMocked();
        var o = m.AnalyzeFile<MResult>(path);

        File.Delete(path);

        Assert.NotNull(o);
        Assert.Equal("value", o.field);
    }

    [Fact]
    public void ImageAnalyzerHttpBaseObjWithException()
    {
        var setting = new Dictionary<string, string>
        {
            {"LPR:lprBaseUrl", "https://localhost"},
            {"LPR:lprPath", "/service"},
            {"LPR:lprAuthKey", "thisisauthkey"}
        };
        var configuration = GetCofigFromDictionary(setting);
    
        var path = "/tmp/test.tmp";
        File.Create(path).Dispose();

        var m = new LPRAnalyzer(configuration);
        var result = m.AnalyzeFile(path);

        Assert.Equal("", result);

        File.Delete(path);
    }
}
