using Serilog;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Text.RegularExpressions;

namespace Prom.LPR.Api.Authentications
{
    public abstract class BasicAuthenticationHandlerBase : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        protected abstract User? Authenticate(string orgId, string username, string password, HttpRequest request);
        private static IConfiguration? cfg = null;

        public BasicAuthenticationHandlerBase(
            IOptionsMonitor<AuthenticationSchemeOptions> options, 
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock) : base(options, logger, encoder, clock)
        {
        }

        public static void SetConfiguration(IConfiguration config)
        {
            cfg = config;
        }

        public IConfiguration? GetConfiguration()
        {
            return cfg;
        }

        private string GetOrgId(HttpRequest request)
        {
            var pattern = @"^\/api\/(.+)\/org\/(.+)\/action\/(.+)$";
            var path = request.Path;
            MatchCollection matches = Regex.Matches(path, pattern);

            var orgId = matches[0].Groups[2].Value;

            return orgId;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey("Authorization"))
            {
                return AuthenticateResult.Fail("No Authorization header found");
            }

            var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
            if (authHeader.Scheme.Equals("Bearer"))
            {
                return AuthenticateResult.NoResult();
            }
            else if (!authHeader.Scheme.Equals("Basic"))
            {
                return AuthenticateResult.Fail($"Unknown scheme [{authHeader.Scheme}]");
            }

            User? user = null;
            try
            {
                var credentialBytes = Convert.FromBase64String(authHeader.Parameter!);
                var credentials = Encoding.UTF8.GetString(credentialBytes).Split(new[] { ':' }, 2);
                var username = credentials[0];
                var password = credentials[1];

                var orgId = GetOrgId(Request);
                user = await Task.Run(() => Authenticate(orgId, username, password, Request));
            }
            catch (Exception e)
            {
                Log.Error($"[BasicAuthenticationHandlerBase] --> [{e.Message}]");
                return AuthenticateResult.Fail("Invalid Authorization Header");
            }

            if (user == null)
            {
                return AuthenticateResult.Fail("Invalid username or password");
            }

            var identity = new ClaimsIdentity(user.claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            Context.Request.Headers.Add("AuthenScheme", "Basic");

            return AuthenticateResult.Success(ticket);
        }
    }
}