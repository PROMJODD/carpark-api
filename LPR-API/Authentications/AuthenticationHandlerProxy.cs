using Serilog;
using System.Text;
using System.Text.Encodings.Web;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Security.Principal;

namespace Prom.LPR.Api.Authentications
{
    public class AuthenticationHandlerProxy : AuthenticationHandlerProxyBase
    {
        private readonly IBasicAuthenticationRepo? basicAuthenRepo = null;
        private readonly IBearerAuthenticationRepo? bearerAuthRepo = null;
        private readonly IConfiguration config;
        private static string? signedKeyJson = null;

        public AuthenticationHandlerProxy(
            IOptionsMonitor<AuthenticationSchemeOptions> options, 
            ILoggerFactory logger,
            UrlEncoder encoder,
            IBasicAuthenticationRepo bsAuthRepo,
            IBearerAuthenticationRepo brAuthRepo,
            IConfiguration cfg,
            ISystemClock clock) : base(options, logger, encoder, clock)
        {
            basicAuthenRepo = bsAuthRepo;
            bearerAuthRepo = brAuthRepo;
            config = cfg;
        }

        protected override User? AuthenticateBasic(string orgId, byte[]? jwtBytes, HttpRequest request)
        {
            var credentials = Encoding.UTF8.GetString(jwtBytes!).Split(new[] { ':' }, 2);
            var username = credentials[0];
            var password = credentials[1];

            var user = basicAuthenRepo!.Authenticate(orgId, username, password, request);
            return user;
        }

        private string GetSignedKeyJson(string? url)
        {
            if (signedKeyJson != null)
            {
                return signedKeyJson;
            }

            Log.Information($"Getting JSON public key from [{url}]");

            var handler = new HttpClientHandler() 
            { 
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };

            var client = new HttpClient(handler)
            {
                Timeout = TimeSpan.FromMinutes(0.05)
            };

            var task = client.GetAsync(url);
            var response = task.Result;
            signedKeyJson = response.Content.ReadAsStringAsync().Result;

            return signedKeyJson;
        }

        protected override User? AuthenticateBearer(string orgId, byte[]? jwtBytes, HttpRequest request)
        {
            var accessToken = Encoding.UTF8.GetString(jwtBytes!);
            var tokenHandler = new JwtSecurityTokenHandler();

            var param = new TokenValidationParameters()
            {
                ValidIssuer = config["SSO:issuer"],
                ValidAudience = config["SSO:audience"],
                IssuerSigningKey = new JsonWebKey(GetSignedKeyJson(config["SSO:signedKeyUrl"])),
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
            };

            SecurityToken validatedToken;
            IPrincipal principal = tokenHandler.ValidateToken(accessToken, param, out validatedToken);
            if (principal.Identity == null || !principal.Identity.IsAuthenticated)
            {
                return null;
            }

            var jwt = tokenHandler.ReadJwtToken(accessToken);
            string userName = jwt.Claims.First(c => c.Type == "preferred_username").Value;

            var user = bearerAuthRepo!.Authenticate(orgId, userName, "", request);

            return user;
        }
    }
}