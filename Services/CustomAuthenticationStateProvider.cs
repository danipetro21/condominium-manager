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
                var claims = await _userManager.GetClaimsAsync(user);
                _currentUser = new ClaimsPrincipal(new ClaimsIdentity(claims, "Custom"));
            }
        }

        return new AuthenticationState(_currentUser);
    }

    public async Task UpdateAuthenticationStateAsync(ApplicationUser? user)
    {
        if (user != null)
        {
            var claims = await _userManager.GetClaimsAsync(user);
            _currentUser = new ClaimsPrincipal(new ClaimsIdentity(claims, "Custom"));
        }
        else
        {
            _currentUser = new ClaimsPrincipal(new ClaimsIdentity());
        }

        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
} 