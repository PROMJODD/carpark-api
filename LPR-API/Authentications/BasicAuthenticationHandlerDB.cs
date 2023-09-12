using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace Prom.LPR.Api.Authentications
{
    public class BasicAuthenticationHandlerDB : BasicAuthenticationHandlerBase
    {
        private readonly IBasicAuthenticationRepo? authenRepo = null;

        public BasicAuthenticationHandlerDB(
            IOptionsMonitor<AuthenticationSchemeOptions> options, 
            ILoggerFactory logger,
            UrlEncoder encoder,
            IBasicAuthenticationRepo authRepo,
            ISystemClock clock) : base(options, logger, encoder, clock)
        {
            authenRepo = authRepo;
        }

        protected override User? Authenticate(string orgId, string username, string password, HttpRequest request)
        {
            var user = authenRepo!.Authenticate(orgId, username, password, request);
            return user;
        }
    }
}