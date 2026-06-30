using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MainSchoolsManagementSystem.Tests
{
    public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public TestAuthHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder)
            : base(options, logger, encoder)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.TryGetValue("X-Test-UserId", out var userIdValues))
            {
                return Task.FromResult(AuthenticateResult.NoResult());
            }

            var userId = userIdValues.ToString();
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Name, "test@example.com")
            };

            if (Request.Headers.TryGetValue("X-Test-Role", out var roleValues))
            {
                foreach (var role in roleValues.ToString().Split(','))
                {
                    claims.Add(new Claim(ClaimTypes.Role, role.Trim()));
                }
            }

            if (Request.Headers.TryGetValue("X-Test-SchoolId", out var schoolIdValues))
            {
                claims.Add(new Claim("SchoolId", schoolIdValues.ToString()));
            }

            var identity = new ClaimsIdentity(claims, "Test");
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, "Test");

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}
