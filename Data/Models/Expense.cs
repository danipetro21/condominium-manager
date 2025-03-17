using System;
using System.ComponentModel.DataAnnotations;

namespace cem.Data.Models
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

        [Required]
        [StringLength(500)]
        public required string Description { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Amount { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        [StringLength(100)]
        public required string Type { get; set; }

        [Required]
        public ExpenseStatus Status { get; set; } = ExpenseStatus.Pending;

        public string? AttachmentPath { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? ApprovedAt { get; set; }

        public string? RejectionReason { get; set; }

        // Relazione con il condominio
        public int CondominiumId { get; set; }
        public required Condominium Condominium { get; set; }

        // Relazione con l'utente che ha creato la spesa
        public int CreatedById { get; set; }
        public required User CreatedBy { get; set; }

        // Relazione con l'utente che ha approvato/rifiutato la spesa
        public int? ApprovedById { get; set; }
        public virtual User? ApprovedBy { get; set; }
    }
} 