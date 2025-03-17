using System;
using System.Collections.Generic;

namespace cem.Data.Models
{
    public class Condominium
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Address { get; set; }
        public required string City { get; set; }
        public required string Province { get; set; }
        public required string PostalCode { get; set; }
        public DateTime CreationDate { get; set; }
        public virtual ICollection<Expense> Expenses { get; set; } = new List<Expense>();
    }
} 