using System;
using System.ComponentModel.DataAnnotations;

namespace cem.Models
{
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
        [StringLength(100)]
        public required string FirstName { get; set; }

        [Required]
        [StringLength(100)]
        public required string LastName { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int RoleId { get; set; }

        public Role Role { get; set; } 

        public virtual ICollection<Condominium> ManagedCondominiums { get; set; } = new List<Condominium>();
    }
}