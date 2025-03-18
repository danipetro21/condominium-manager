using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using cem.Data;
using cem.Models;
using Microsoft.EntityFrameworkCore;
using cem.Utilities;
using Microsoft.Extensions.Logging;

namespace cem.Services
{
    public class AuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            ApplicationDbContext context, 
            AuthenticationStateProvider authenticationStateProvider,
            ILogger<AuthService> logger)
        {
            _context = context;
            _authenticationStateProvider = authenticationStateProvider;
            _logger = logger;
        }

        public async Task<bool> LoginAsync(string email, string password)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email == email);

            if (user != null && PasswordHasher.VerifyPassword(password, user.PasswordHash))
            {
                var identity = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role?.Name ?? "Manager"),
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim("FirstName", user.FirstName),
                    new Claim("LastName", user.LastName)
                }, "apiauth_type");

                var claimsPrincipal = new ClaimsPrincipal(identity);
                ((CustomAuthenticationStateProvider)_authenticationStateProvider).MarkUserAsAuthenticated(claimsPrincipal);

                return true;
            }

            return false;
        }

        public async Task LogoutAsync()
        {
            ((CustomAuthenticationStateProvider)_authenticationStateProvider).MarkUserAsLoggedOut();
        }
    }

    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        private ClaimsPrincipal _currentUser = new ClaimsPrincipal(new ClaimsIdentity());

        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            return Task.FromResult(new AuthenticationState(_currentUser));
        }

        public void MarkUserAsAuthenticated(ClaimsPrincipal user)
        {
            _currentUser = user;
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        public void MarkUserAsLoggedOut()
        {
            _currentUser = new ClaimsPrincipal(new ClaimsIdentity());
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }
    }
}