using System.ComponentModel.DataAnnotations;

namespace cem.Models
{
    public enum NotificationType
    {
        ExpenseApproved,
        ExpenseRejected,
        PaymentDue,
        SystemNotification
    }

    public class Notification
    {
        public int Id { get; set; }

        [Required]
        public required string Title { get; set; }

        [Required]
        public required string Message { get; set; }

        [Required]
        public NotificationType Type { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? ReadAt { get; set; }

        // Relazione con l'utente destinatario
        public string UserId { get; set; } = string.Empty;
        public required ApplicationUser User { get; set; }

        // Relazione con la spesa (se la notifica è relativa a una spesa)
        public int? ExpenseId { get; set; }
        public virtual Expense? Expense { get; set; }
    }
}