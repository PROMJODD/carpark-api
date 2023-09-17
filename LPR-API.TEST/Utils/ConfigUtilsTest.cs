using Xunit;
using Prom.LPR.Api.Utils;
using Microsoft.Extensions.Configuration;

namespace Prom.LPR.Test.Api.Utils;

public class ConfigUtilsTest
{
    private IConfiguration GetCofigFromDictionary(Dictionary<string, string> setting)
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(setting!)
            .Build();

        return configuration;
    }

    [Theory]
    [InlineData("Key1", "CfgValue1", "Key1", "CfgValue1")]
    [InlineData("Key1:Field1", "CfgValue1", "Key1:Field1", "CfgValue1")]
    [InlineData("Key1:Field1", "CfgValue1", "Key1:Field2", "")]
    public void ReturnnValueWithoutEnv(string cfgKey, string cfgValue, string getKey, string expectedValue)
    {
        var setting = new Dictionary<string, string>
        {
            {cfgKey, cfgValue},
        };
        var configuration = GetCofigFromDictionary(setting);

        var value = ConfigUtils.GetConfig(configuration, getKey);
        Assert.Equal(expectedValue, value);
    }

    [Theory]
    [InlineData("Key1:Abc", "CfgValue1", "Key1__Abc", "EnvValue1", "EnvValue1")]
    [InlineData("Key1:Abc", "CfgValue1", "Key1__Abc", "CfgValue1", "CfgValue1")]
    public void ReturnValueWithEnv(string cfgKey, string cfgValue, string envKey, string envValue, string expectedValue)
    {
        Environment.SetEnvironmentVariable(envKey, envValue);
        var setting = new Dictionary<string, string>
        {
            {cfgKey, cfgValue},
        };
        var configuration = GetCofigFromDictionary(setting);

        var value = ConfigUtils.GetConfig(configuration, cfgKey);
        Assert.Equal(expectedValue, value);
    }

    [Theory]
    [InlineData("Key1:Abc", "Key1__Abc", "EnvValueA", "EnvValueA")]
    [InlineData("Key1:Abc", "Key1__Abc", "CfgValueB", "CfgValueB")]
    public void ReturnValueWithEnvEvenConfigNull(string cfgKey, string envKey, string envValue, string expectedValue)
    {
        Environment.SetEnvironmentVariable(envKey, envValue);

        var value = ConfigUtils.GetConfig(null!, cfgKey);
        Assert.Equal(expectedValue, value);
    }

    [Theory]
    [InlineData("Key1:Abc", "")]
    [InlineData("Key1", "")]
    public void ReturnValueWithoutEnvAndConfigNull(string cfgKey, string expectedValue)
    {
        var value = ConfigUtils.GetConfig(null!, cfgKey);
        Assert.Equal(expectedValue, value);
    }
}
