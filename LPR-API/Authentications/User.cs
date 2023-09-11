using System.Security.Claims;

namespace Prom.LPR.Api.Authentications
{
    public class User
    {
        public Guid? UserId {get; set;}
        public string? UserName {get; set;}
        public string? Password {get; set;}
        public string? Role {get; set;}
        public string? AuthenType {get; set;}
        public IEnumerable<Claim>? claims = null;
    }
}
