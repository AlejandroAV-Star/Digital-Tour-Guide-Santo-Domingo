using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;

namespace DTourGuide.API.Auth
{
    public class BasicAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public const string SchemeName = "BasicAdmin";

        public BasicAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
            : base(options, logger, encoder, clock) { }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey("Authorization"))
                return Task.FromResult(AuthenticateResult.Fail("Missing Authorization"));

            try
            {
                var header = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
                if (!"Basic".Equals(header.Scheme, StringComparison.OrdinalIgnoreCase))
                    return Task.FromResult(AuthenticateResult.Fail("Invalid Scheme"));

                var credentialBytes = Convert.FromBase64String(header.Parameter!);
                var credentials = Encoding.UTF8.GetString(credentialBytes).Split(':', 2);
                var user = credentials[0]; var pass = credentials[1];

                var cfgUser = Context.RequestServices.GetRequiredService<IConfiguration>()["Admin:User"];
                var cfgPass = Context.RequestServices.GetRequiredService<IConfiguration>()["Admin:Password"];

                if (user == cfgUser && pass == cfgPass)
                {
                    var claims = new[] { new Claim(ClaimTypes.Name, user), new Claim(ClaimTypes.Role, "Admin") };
                    var identity = new ClaimsIdentity(claims, SchemeName);
                    var principal = new ClaimsPrincipal(identity);
                    var ticket = new AuthenticationTicket(principal, SchemeName);
                    return Task.FromResult(AuthenticateResult.Success(ticket));
                }
                return Task.FromResult(AuthenticateResult.Fail("Invalid Credentials"));
            }
            catch
            {
                return Task.FromResult(AuthenticateResult.Fail("Invalid Authorization Header"));
            }
        }
    }
}
