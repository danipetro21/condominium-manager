using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace cem.Data.Models
{
    public class Condominium
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public required string Name { get; set; }

        [Required]
        [StringLength(500)]
        public required string Address { get; set; }

        [Required]
        [StringLength(100)]
        public required string City { get; set; }

        [Required]
        [StringLength(2)]
        public required string Province { get; set; }

        [Required]
        [StringLength(5)]
        public required string PostalCode { get; set; }

        public DateTime CreationDate { get; set; }

        // Relazione con le spese
        public virtual ICollection<Expense> Expenses { get; set; } = new List<Expense>();

        // Relazione con i gestori del condominio
        public virtual ICollection<User> Managers { get; set; } = new List<User>();
    }
} 