using Xunit;
using Prom.LPR.Api.Authentications;

namespace Prom.LPR.Test.Api.Authentications;

public class JwtSignerTest
{

    [Theory]
    [InlineData("https://nohost")]
    [InlineData("")]
    public void ExceptionWhenUrlNotValid(string url)
    {
        var sn = new JwtSigner();
        JwtSigner.ResetSigedKeyJson();

        Action act = () => sn.GetSignedKey(url);

        var ex = Record.Exception(act);
        Assert.NotNull(ex);
    }

    [Theory]
    [InlineData("https://google.com")]
    public void ExceptionWhenUrlNotForOauth(string url)
    {
        var sn = new JwtSigner();
        JwtSigner.ResetSigedKeyJson();

        Action act = () => sn.GetSignedKey(url);

        var ex = Record.Exception(act);
        Assert.NotNull(ex);
    }

    [Theory]
    [InlineData("https://keycloak.promid.prom.co.th/auth/realms/promid/protocol/openid-connect/certs")]
    public void SuccessWhenUrlValidForOauth(string url)
    {
        var sn = new JwtSigner();

        JwtSigner.ResetSigedKeyJson();
        var key1 = sn.GetSignedKey(url);
        var key2 = sn.GetSignedKey(url); //The 2nd calll to get the one from static field

        Assert.NotNull(key1);
        Assert.NotNull(key2);
    }
}
