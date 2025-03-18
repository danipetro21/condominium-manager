using System.ComponentModel.DataAnnotations;

namespace cem.Models
{
    public class Role
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public required string Name { get; set; }

        public virtual ICollection<User> Users { get; set; } = new List<User>();
    }
}