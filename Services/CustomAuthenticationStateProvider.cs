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
    private ApplicationUser? _cachedUser;
    private bool _isInitialized;

    public CustomAuthenticationStateProvider(
        UserManager<ApplicationUser> userManager,
        IHttpContextAccessor httpContextAccessor)
    {
        _userManager = userManager;
        _httpContextAccessor = httpContextAccessor;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        if (_isInitialized)
        {
            return new AuthenticationState(_currentUser);
        }

        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext?.User?.Identity?.IsAuthenticated == true)
        {
            try
            {
                if (_cachedUser == null)
                {
                    _cachedUser = await _userManager.GetUserAsync(httpContext.User);
                }

                if (_cachedUser != null)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, _cachedUser.Id),
                        new Claim(ClaimTypes.Name, _cachedUser.UserName ?? string.Empty),
                        new Claim(ClaimTypes.Email, _cachedUser.Email ?? string.Empty)
                    };

                    var roles = await _userManager.GetRolesAsync(_cachedUser);
                    foreach (var role in roles)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, role));
                    }

                    _currentUser = new ClaimsPrincipal(new ClaimsIdentity(claims, "Custom"));
                }
            }
            catch (Exception)
            {
                // In caso di errore, manteniamo l'utente non autenticato
                _currentUser = new ClaimsPrincipal(new ClaimsIdentity());
            }
        }
        else
        {
            _cachedUser = null;
            _currentUser = new ClaimsPrincipal(new ClaimsIdentity());
        }

        _isInitialized = true;
        return new AuthenticationState(_currentUser);
    }

    public async Task UpdateAuthenticationStateAsync(ApplicationUser? user)
    {
        _cachedUser = user;
        _isInitialized = false;

        if (user != null)
        {
            try
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
                    new Claim(ClaimTypes.Email, user.Email ?? string.Empty)
                };

                var roles = await _userManager.GetRolesAsync(user);
                foreach (var role in roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }

                _currentUser = new ClaimsPrincipal(new ClaimsIdentity(claims, "Custom"));
            }
            catch (Exception)
            {
                _currentUser = new ClaimsPrincipal(new ClaimsIdentity());
            }
        }
        else
        {
            _currentUser = new ClaimsPrincipal(new ClaimsIdentity());
        }

        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
}