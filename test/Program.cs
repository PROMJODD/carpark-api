using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;

var env = ""; // "-dev"
var realms = "promid-nonprod";

var jwtStr = "eyJhbGciOiJSUzI1NiIsInR5cCIgOiAiSldUIiwia2lkIiA6ICJIcWlRY2ZXZTM2UkE4WTRnOXk4YzBxMzZZajlaa2pQby12Q0NBQ2o0eE5RIn0.eyJleHAiOjE3MDUzNjc5MjksImlhdCI6MTcwNTM2NzYyOSwianRpIjoiMGExYmUwZGUtOTBmMy00ZWE5LTg0YmMtZjk5NTBmN2Q1NWNiIiwiaXNzIjoiaHR0cHM6Ly9rZXljbG9hay5wcm9taWQucHJvbS5jby50aC9hdXRoL3JlYWxtcy9wcm9taWQtbm9ucHJvZCIsImF1ZCI6ImFjY291bnQiLCJzdWIiOiIwNzQ1NTU5Ny0zMWJiLTQ5OWEtOTg4ZS05YWE5NTU3ODVkOTkiLCJ0eXAiOiJCZWFyZXIiLCJhenAiOiJwcm9tam9kZCIsInNlc3Npb25fc3RhdGUiOiI2M2VmYTY4Mi02YTRlLTQwNTYtYWMxMS0yMThhOGU5ODJmZmQiLCJhY3IiOiIxIiwicmVhbG1fYWNjZXNzIjp7InJvbGVzIjpbIm9mZmxpbmVfYWNjZXNzIiwidW1hX2F1dGhvcml6YXRpb24iLCJkZWZhdWx0LXJvbGVzLXByb21pZC1ub25wcm9kIl19LCJyZXNvdXJjZV9hY2Nlc3MiOnsiYWNjb3VudCI6eyJyb2xlcyI6WyJtYW5hZ2UtYWNjb3VudCIsIm1hbmFnZS1hY2NvdW50LWxpbmtzIiwidmlldy1wcm9maWxlIl19fSwic2NvcGUiOiJvcGVuaWQgcHJvZmlsZSBlbWFpbCIsInNpZCI6IjYzZWZhNjgyLTZhNGUtNDA1Ni1hYzExLTIxOGE4ZTk4MmZmZCIsImVtYWlsX3ZlcmlmaWVkIjpmYWxzZSwibmFtZSI6IlRpcCBUaXAiLCJwcmVmZXJyZWRfdXNlcm5hbWUiOiJ0cHRoZW1lc0BnbWFpbC5jb20iLCJnaXZlbl9uYW1lIjoiVGlwIiwiZmFtaWx5X25hbWUiOiJUaXAiLCJlbWFpbCI6InRwdGhlbWVzQGdtYWlsLmNvbSJ9.oU4txgeBK9w_kuP8T1kd-S2n_Uj14Ba7tEjkQKKWXtN5VkKMcFuNCKGVYEKRgrP6J0JIdGT1WAWtC8iWDLRFiv9JmwUolSiyyteGQkc4I3aChjIxznFOgoDY3q1SG0IbtqQ05rymfdZ-H1f-S2I9Gd981R9B6T3tsOVY7fgM9azWSgcPJFT2t-NQJPDkFk8Qymtex34EJggNlVGPrkDQEAHqXlbPUiqHOFod5xbbpuTqCM1v9D7SkCLxvdDGiMNCm5zQTEJVkqcikAbqvp-zxoeGvemLPxgWRJG3FAmWsqom8FWsJ6NGokqw__LBdfZDRMT16xGP9DFZwNzUJbwSmA";

//"eyJhbGciOiJSUzI1NiIsInR5cCIgOiAiSldUIiwia2lkIiA6ICJIcWlRY2ZXZTM2UkE4WTRnOXk4YzBxMzZZajlaa2pQby12Q0NBQ2o0eE5RIn0.eyJleHAiOjE3MDUzNjYxMzEsImlhdCI6MTcwNTM2NTgzMSwianRpIjoiYjExYzFkZjItMWJkMC00ZjA1LWJhMWItOTIxNGMxMmNjZDU5IiwiaXNzIjoiaHR0cHM6Ly9rZXljbG9hay5wcm9taWQucHJvbS5jby50aC9hdXRoL3JlYWxtcy9wcm9taWQtbm9ucHJvZCIsImF1ZCI6ImFjY291bnQiLCJzdWIiOiIwNzQ1NTU5Ny0zMWJiLTQ5OWEtOTg4ZS05YWE5NTU3ODVkOTkiLCJ0eXAiOiJCZWFyZXIiLCJhenAiOiJwcm9tam9kZCIsInNlc3Npb25fc3RhdGUiOiI2ZmZjZjRkZS1jODJhLTQ2ODctYjMyNy01MDI4NTY3YTkzZWIiLCJhY3IiOiIxIiwicmVhbG1fYWNjZXNzIjp7InJvbGVzIjpbIm9mZmxpbmVfYWNjZXNzIiwidW1hX2F1dGhvcml6YXRpb24iLCJkZWZhdWx0LXJvbGVzLXByb21pZC1ub25wcm9kIl19LCJyZXNvdXJjZV9hY2Nlc3MiOnsiYWNjb3VudCI6eyJyb2xlcyI6WyJtYW5hZ2UtYWNjb3VudCIsIm1hbmFnZS1hY2NvdW50LWxpbmtzIiwidmlldy1wcm9maWxlIl19fSwic2NvcGUiOiJvcGVuaWQgcHJvZmlsZSBlbWFpbCIsInNpZCI6IjZmZmNmNGRlLWM4MmEtNDY4Ny1iMzI3LTUwMjg1NjdhOTNlYiIsImVtYWlsX3ZlcmlmaWVkIjpmYWxzZSwibmFtZSI6IlRpcCBUaXAiLCJwcmVmZXJyZWRfdXNlcm5hbWUiOiJ0cHRoZW1lc0BnbWFpbC5jb20iLCJnaXZlbl9uYW1lIjoiVGlwIiwiZmFtaWx5X25hbWUiOiJUaXAiLCJlbWFpbCI6InRwdGhlbWVzQGdtYWlsLmNvbSJ9.nTaiiFjzbxNV4Ea2Gbcc6dKQMnnRTCG7GJU6xSZ3mkpevq2r8QdAw3bW3w19TtAp_nWM-ObzcQWUXCs3qQrSKng_HSE8UB2jGu3nkzcY_fG2TgBmEjc27zBIqkNFo-y1t2VWMmi0BgPDT6VVZRtJa1MTf-iJXCCrfL4BPtmVuon5I4RV3LkZLcO3x5zVsZWm_iE7eTIcW0u5K5Cpy-9PClVjvkUOan4v9kM5MaHuWjr-P4_TxqrV13VemLre-VsLOPrgDns-WG6EZeKnIHckqA0j7moBfvW4PhpSwNCxLkGDu_u-HtHs4uYCCyhbzKSVcxmtwZEzFErcjcVGTyhvxg";
var credentialBytes = Convert.FromBase64String(Base64Encode(jwtStr));
var signedKeyUrl = $"https://keycloak.promid{env}.prom.co.th/auth/realms/{realms}/protocol/openid-connect/certs";

var accessToken = Encoding.UTF8.GetString(credentialBytes!);
var tokenHandler = new JwtSecurityTokenHandler();

var param = new TokenValidationParameters()
{
    ValidIssuer = $"https://keycloak.promid{env}.prom.co.th/auth/realms/{realms}",
    ValidAudience = "account",
    IssuerSigningKey = new JsonWebKey(GetSignedKeyJson(signedKeyUrl)),
    ValidateIssuer = true,
    ValidateAudience = true,
    ValidateLifetime = false, //true
    ValidateIssuerSigningKey = true,
};

SecurityToken validatedToken;
tokenHandler.ValidateToken(accessToken, param, out validatedToken);
var jwt = tokenHandler.ReadJwtToken(accessToken);
string userName = jwt.Claims.First(c => c.Type == "preferred_username").Value;

Console.WriteLine("User name is [{0}]", userName);

string GetSignedKeyJson(string? url)
{
    var handler = new HttpClientHandler() 
    {
    };

    var client = new HttpClient(handler)
    {
        Timeout = TimeSpan.FromMinutes(0.05)
    };

    var task = client.GetAsync(url);
    var response = task.Result;
    var signedKeyJson = response.Content.ReadAsStringAsync().Result;

    return signedKeyJson;
}

string Base64Encode(string plainText) 
{
  var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
  return Convert.ToBase64String(plainTextBytes);
}