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

        public int CondominiumId { get; set; }
        public required Condominium Condominium { get; set; }

        public string CreatedById { get; set; } = string.Empty;
        public required ApplicationUser CreatedBy { get; set; }

        public string? ApprovedById { get; set; }
        public virtual ApplicationUser? ApprovedBy { get; set; }
    }
} 