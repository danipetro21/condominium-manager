using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using cem.Models;
using cem.Data;

namespace cem.Services;

public class UserService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ILogger<UserService> _logger;

    public UserService(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        ILogger<UserService> logger)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _logger = logger;
    }

    public async Task<List<ApplicationUser>> GetUsersAsync()
    {
        return await _userManager.Users.ToListAsync();
    }

    public async Task<ApplicationUser?> GetUserAsync(string id)
    {
        return await _userManager.FindByIdAsync(id);
    }

    public async Task<List<string>> GetUserRolesAsync(ApplicationUser user)
    {
        return (await _userManager.GetRolesAsync(user)).ToList();
    }

    public async Task<List<string>> GetAllRolesAsync()
    {
        var roles = await _roleManager.Roles
            .Select(r => r.Name)
            .Where(n => n != null)
            .ToListAsync();
        return roles!;
    }

    public async Task<(bool Succeeded, string[] Errors)> CreateUserAsync(ApplicationUser user, string password, string role)
    {
        var result = await _userManager.CreateAsync(user, password);
        if (!result.Succeeded)
        {
            return (false, result.Errors.Select(e => e.Description).ToArray());
        }

        var roleResult = await _userManager.AddToRoleAsync(user, role);
        if (!roleResult.Succeeded)
        {
            // Rollback user creation
            await _userManager.DeleteAsync(user);
            return (false, roleResult.Errors.Select(e => e.Description).ToArray());
        }

        return (true, Array.Empty<string>());
    }

    public async Task<(bool Succeeded, string[] Errors)> UpdateUserAsync(ApplicationUser user, string? newRole = null)
    {
        var currentRoles = await _userManager.GetRolesAsync(user);
        var result = await _userManager.UpdateAsync(user);
        
        if (!result.Succeeded)
        {
            return (false, result.Errors.Select(e => e.Description).ToArray());
        }

        if (newRole != null && !currentRoles.Contains(newRole))
        {
            // Rimuovi tutti i ruoli esistenti
            await _userManager.RemoveFromRolesAsync(user, currentRoles);
            // Aggiungi il nuovo ruolo
            var roleResult = await _userManager.AddToRoleAsync(user, newRole);
            if (!roleResult.Succeeded)
            {
                return (false, roleResult.Errors.Select(e => e.Description).ToArray());
            }
        }

        return (true, Array.Empty<string>());
    }

    public async Task<(bool Succeeded, string[] Errors)> DeleteUserAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return (false, new[] { "Utente non trovato." });
        }

        var result = await _userManager.DeleteAsync(user);
        return (result.Succeeded, result.Errors.Select(e => e.Description).ToArray());
    }
} 