using Microsoft.AspNetCore.Identity;
using cem.Models;

namespace cem.Services;

public class AuthService
{
    private readonly SignInManager<ApplicationUser> _signInManager;

    public AuthService(SignInManager<ApplicationUser> signInManager)
    {
        _signInManager = signInManager;
    }

    public async Task LoginAsync(ApplicationUser user)
    {
        await _signInManager.SignInAsync(user, isPersistent: false);
    }

    public async Task LogoutAsync()
    {
        await _signInManager.SignOutAsync();
    }
}