using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace cem.Models;

public class UserCondominium
{
    [Key]
    public string ManagersId { get; set; } = string.Empty;
    
    [Key]
    public int ManagedCondominiumsId { get; set; }

    [ForeignKey("ManagersId")]
    public ApplicationUser Manager { get; set; } = null!;

    [ForeignKey("ManagedCondominiumsId")]
    public Condominium ManagedCondominium { get; set; } = null!;
} 