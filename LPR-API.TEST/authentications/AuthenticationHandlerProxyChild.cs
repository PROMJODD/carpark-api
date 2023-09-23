using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Prom.LPR.Api.Authentications;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace Prom.LPR.Test.Api.Authentications
{
    public class AuthenticationHandlerProxyChild : AuthenticationHandlerProxy
    {
        public AuthenticationHandlerProxyChild(
            IOptionsMonitor<AuthenticationSchemeOptions> options, 
            ILoggerFactory logger,
            UrlEncoder encoder,
            IBasicAuthenticationRepo bsAuthRepo,
            IBearerAuthenticationRepo brAuthRepo,
            IConfiguration cfg,
            ISystemClock clock) : base(options, logger, encoder, bsAuthRepo, brAuthRepo, cfg, clock)
        {
        }

        public Task<AuthenticateResult> HandleAuthenticateAsyncWrap()
        {
            var t = HandleAuthenticateAsync();
            return t;
        }
    }
}
