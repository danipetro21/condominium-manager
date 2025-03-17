using System.ComponentModel.DataAnnotations;

namespace cem.Data.Models
{
    public enum UserRole
    {
        Admin,
        CondominiumManager
    }

    public class User
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public required string Username { get; set; }

        [Required]
        [StringLength(100)]
        public required string Email { get; set; }

        [Required]
        public required string PasswordHash { get; set; }

        [Required]
        public UserRole Role { get; set; }

        [Required]
        [StringLength(100)]
        public required string FirstName { get; set; }

        [Required]
        [StringLength(100)]
        public required string LastName { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Relazione con i condomini gestiti (solo per i CondominiumManager)
        public virtual ICollection<Condominium> ManagedCondominiums { get; set; } = new List<Condominium>();
    }
} 