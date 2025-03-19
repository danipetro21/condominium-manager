using System;
using System.ComponentModel.DataAnnotations;

namespace cem.Models
{
    public enum ExpenseStatus
    {
        Pending,
        Approved,
        Rejected
    }

    public class Expense
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "La descrizione è obbligatoria")]
        [StringLength(500, ErrorMessage = "La descrizione non può superare i 500 caratteri")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "L'importo è obbligatorio")]
        [Range(0.01, double.MaxValue, ErrorMessage = "L'importo deve essere maggiore di zero")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "La data è obbligatoria")]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "La categoria è obbligatoria")]
        public ExpenseCategory Category { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ApprovedAt { get; set; }
        public ExpenseStatus Status { get; set; } = ExpenseStatus.Pending;

        // Relazioni
        public string CreatedById { get; set; } = string.Empty;
        public ApplicationUser CreatedBy { get; set; } = null!;
        public string? ApprovedById { get; set; }
        public ApplicationUser? ApprovedBy { get; set; }
        public int CondominiumId { get; set; }
        public Condominium Condominium { get; set; } = null!;
        
        // Relazione con i file
        public virtual ICollection<AppFile> Files { get; set; } = new List<AppFile>();
    }

    public enum ExpenseCategory
    {
        Manutenzione,
        Pulizia,
        Energia,
        Assicurazione,
        Altro
    }
} 