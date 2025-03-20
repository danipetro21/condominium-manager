using Microsoft.EntityFrameworkCore;
using cem.Data;
using cem.Models;

namespace cem.Services;

public class NotificationService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(ApplicationDbContext context, ILogger<NotificationService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<Notification>> GetAllNotificationsAsync()
    {
        try
        {
            return await _context.Notifications
                .Include(n => n.User)
                .Include(n => n.Expense)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Errore durante il recupero di tutte le notifiche");
            throw;
        }
    }

    public async Task<List<Notification>> GetUserNotificationsAsync(string userId)
    {
        try
        {
            return await _context.Notifications
                .Include(n => n.User)
                .Include(n => n.Expense)
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Errore durante il recupero delle notifiche dell'utente {UserId}", userId);
            throw;
        }
    }

    public async Task<Notification?> GetNotificationAsync(int id)
    {
        try
        {
            return await _context.Notifications
                .Include(n => n.User)
                .Include(n => n.Expense)
                .FirstOrDefaultAsync(n => n.Id == id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Errore durante il recupero della notifica {NotificationId}", id);
            throw;
        }
    }

    public async Task MarkAsReadAsync(int id)
    {
        try
        {
            var notification = await _context.Notifications.FindAsync(id);
            if (notification != null && notification.ReadAt == null)
            {
                notification.ReadAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Errore durante la marcatura come letta della notifica {NotificationId}", id);
            throw;
        }
    }

    public async Task DeleteNotificationAsync(int id)
    {
        try
        {
            var notification = await _context.Notifications.FindAsync(id);
            if (notification != null)
            {
                _context.Notifications.Remove(notification);
                await _context.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Errore durante l'eliminazione della notifica {NotificationId}", id);
            throw;
        }
    }

    public async Task<Notification> CreateNotificationAsync(string title, string message, NotificationType type, string userId, int? expenseId = null)
    {
        try
        {
            var user = await _context.Users.FindAsync(userId) ?? throw new InvalidOperationException("Utente non trovato");

            var notification = new Notification
            {
                Title = title,
                Message = message,
                Type = type,
                UserId = userId,
                User = user,
                ExpenseId = expenseId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
            return notification;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Errore durante la creazione della notifica per l'utente {UserId}", userId);
            throw;
        }
    }
}