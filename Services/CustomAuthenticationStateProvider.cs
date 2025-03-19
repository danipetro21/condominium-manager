using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using cem.Models;

namespace cem.Services;

public class CustomAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private ClaimsPrincipal _currentUser = new(new ClaimsIdentity());

    public CustomAuthenticationStateProvider(
        UserManager<ApplicationUser> userManager,
        IHttpContextAccessor httpContextAccessor)
    {
        _userManager = userManager;
        _httpContextAccessor = httpContextAccessor;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext?.User?.Identity?.IsAuthenticated == true)
        {
            var user = await _userManager.GetUserAsync(httpContext.User);
            if (user != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
                    new Claim(ClaimTypes.Email, user.Email ?? string.Empty)
                };

                var roles = await _userManager.GetRolesAsync(user);
                foreach (var role in roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }

                var existingClaims = await _userManager.GetClaimsAsync(user);
                await _userManager.RemoveClaimsAsync(user, existingClaims);
                await _userManager.AddClaimsAsync(user, claims);

                _currentUser = new ClaimsPrincipal(new ClaimsIdentity(claims, "Custom"));
            }
        }

        return new AuthenticationState(_currentUser);
    }

    public async Task UpdateAuthenticationStateAsync(ApplicationUser? user)
    {
        if (user != null)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty)
            };

            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            // Salva le claims nel database
            var existingClaims = await _userManager.GetClaimsAsync(user);
            await _userManager.RemoveClaimsAsync(user, existingClaims);
            await _userManager.AddClaimsAsync(user, claims);

            _currentUser = new ClaimsPrincipal(new ClaimsIdentity(claims, "Custom"));
        }
        else
        {
            _currentUser = new ClaimsPrincipal(new ClaimsIdentity());
        }

        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
} 