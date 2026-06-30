using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;

namespace MainSchoolsManagementSystem.Tests
{
    public class TestAuthenticationStateProvider : AuthenticationStateProvider
    {
        private ClaimsPrincipal _currentPrincipal = new ClaimsPrincipal(new ClaimsIdentity());

        public void SetUser(string username, string role, string userId, int? schoolId = null)
        {
            var identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, role),
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim("SchoolId", schoolId?.ToString() ?? "")
            }, "TestAuth");

            _currentPrincipal = new ClaimsPrincipal(identity);
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_currentPrincipal)));
        }

        public void ClearUser()
        {
            _currentPrincipal = new ClaimsPrincipal(new ClaimsIdentity());
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_currentPrincipal)));
        }

        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            return Task.FromResult(new AuthenticationState(_currentPrincipal));
        }
    }
}
