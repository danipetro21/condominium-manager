using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace cem.Models;

public class ApplicationUser : IdentityUser
{
    [Required]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    public string LastName { get; set; } = string.Empty;

    public virtual ICollection<Condominium> ManagedCondominiums { get; set; } = new List<Condominium>();
    public virtual ICollection<Expense> CreatedExpenses { get; set; } = new List<Expense>();
    public virtual ICollection<Expense> ApprovedExpenses { get; set; } = new List<Expense>();
    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    public virtual ICollection<AppFile> Files { get; set; } = new List<AppFile>();
}