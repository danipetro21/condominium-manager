using System;

namespace cem.Data.Models
{
    public class Expense
    {
        public int Id { get; set; }
        public required string Description { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public required string Type { get; set; }
        public int CondominiumId { get; set; }
        public required Condominium Condominium { get; set; }
    }
} 