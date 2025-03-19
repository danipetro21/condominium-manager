using Microsoft.EntityFrameworkCore;
using cem.Data;
using cem.Models;

namespace cem.Services;

public class CondominiumService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<CondominiumService> _logger;
    private readonly string _uploadPath;
    private readonly EmailService _emailService;

    public CondominiumService(
        ApplicationDbContext context,
        ILogger<CondominiumService> logger,
        IWebHostEnvironment environment)
    {
        _context = context;
        _logger = logger;
        _uploadPath = Path.Combine(environment.WebRootPath ?? "wwwroot", "uploads");
    }

    public async Task<List<Condominium>> GetUserCondominiumsAsync(string userId, bool isAdmin = false)
    {
        try
        {
            var query = _context.UserCondominiums
                .AsNoTracking()
                .Include(uc => uc.ManagedCondominium)
                .AsQueryable();

            if (!isAdmin)
            {
                query = query.Where(uc => uc.ManagersId == userId);
            }

            return await query.Select(uc => uc.ManagedCondominium).ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Errore durante il caricamento dei condomini per l'utente {userId}");
            throw;
        }
    }

    public async Task<Condominium?> GetCondominiumAsync(int id, string userId, bool isAdmin = false)
    {
        try
        {
            var query = _context.UserCondominiums
                .AsNoTracking()
                .Include(uc => uc.ManagedCondominium)
                .Where(uc => uc.ManagedCondominiumsId == id)
                .AsQueryable();

            if (!isAdmin)
            {
                query = query.Where(uc => uc.ManagersId == userId);
            }

            return await query.Select(uc => uc.ManagedCondominium).FirstOrDefaultAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Errore durante il caricamento del condominio {id} per l'utente {userId}");
            throw;
        }
    }

    public async Task<CondominiumSummary> GetCondominiumSummaryAsync(int id, string userId, bool isAdmin = false)
    {
        try
        {
            var condominium = await GetCondominiumAsync(id, userId, isAdmin);
            if (condominium == null)
            {
                throw new ArgumentException("Condominio non trovato");
            }

            var expenses = await _context.Expenses
                .AsNoTracking()
                .Where(e => e.CondominiumId == id)
                .ToListAsync();

            var totalExpenses = expenses.Where(e => e.Status == ExpenseStatus.Approved).Sum(e => e.Amount);
            var expensesByCategory = expenses
                .Where(e => e.Status == ExpenseStatus.Approved)
                .GroupBy(e => e.Category)
                .Select(g => new CategorySummary
                {
                    Category = g.Key,
                    Total = g.Sum(e => e.Amount),
                    Count = g.Count()
                })
                .ToList();

            return new CondominiumSummary
            {
                Condominium = condominium,
                TotalExpenses = totalExpenses,
                ExpensesByCategory = expensesByCategory,
                LastExpenseDate = expenses.Any() ? expenses.Max(e => e.Date) : null,
                PendingExpenses = expenses.Count(e => e.Status == ExpenseStatus.Pending),
                RejectedExpenses = expenses.Count(e => e.Status == ExpenseStatus.Rejected)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Errore durante il calcolo del riepilogo del condominio {id}");
            throw;
        }
    }

    public async Task<List<Expense>> GetPendingExpensesAsync(int condominiumId, string userId, bool isAdmin = false)
    {
        try
        {
            var query = _context.Expenses
                .AsNoTracking()
                .Include(e => e.CreatedBy)
                .Include(e => e.Condominium)
                .Where(e => e.CondominiumId == condominiumId && e.Status == ExpenseStatus.Pending)
                .AsQueryable();

            if (!isAdmin)
            {
                query = query.Where(e => e.CreatedById == userId);
            }

            return await query.OrderByDescending(e => e.Date).ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Errore durante il caricamento delle spese in attesa per il condominio {condominiumId}");
            throw;
        }
    }

    public async Task<Expense> ApproveExpenseAsync(int expenseId, string approverId)
    {
        try
        {
            var expense = await _context.Expenses.FindAsync(expenseId);
            if (expense == null)
            {
                throw new ArgumentException("Spesa non trovata");
            }

            expense.Status = ExpenseStatus.Approved;
            expense.ApprovedById = approverId;
            expense.ApprovedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            var templatePath = Path.Combine("wwwroot", "email", "expense-approved.html");
            var template = await File.ReadAllTextAsync(templatePath);
            var emailBody = template
                .Replace("{{FirstName}}", expense.CreatedBy.FirstName)
                .Replace("{{LastName}}", expense.CreatedBy.LastName)
                .Replace("{{Amount}}", expense.Amount.ToString("N2"))
                .Replace("{{Description}}", expense.Description)
                .Replace("{{CondominiumName}}", expense.Condominium.Name)
                .Replace("{{Date}}", expense.Date.ToString("dd/MM/yyyy"))
                .Replace("{{ApplicationUrl}}", Environment.GetEnvironmentVariable("WWW_DOMAIN") + "/expenses/" + expense.Id);
            await _emailService.SendEmailAsync(expense.CreatedBy.Email!, "Spesa Approvata - Condominium Manager", emailBody );

            return expense;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Errore durante l'approvazione della spesa {expenseId}");
            throw;
        }
    }

    public async Task<Expense> RejectExpenseAsync(int expenseId, string approverId)
    {
        try
        {
            var expense = await _context.Expenses.FindAsync(expenseId);
            if (expense == null)
            {
                throw new ArgumentException("Spesa non trovata");
            }

            expense.Status = ExpenseStatus.Rejected;
            expense.ApprovedById = approverId;
            expense.ApprovedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return expense;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Errore durante il rifiuto della spesa {expenseId}");
            throw;
        }
    }
}

public class CondominiumSummary
{
    public Condominium Condominium { get; set; } = null!;
    public decimal TotalExpenses { get; set; }
    public List<CategorySummary> ExpensesByCategory { get; set; } = new();
    public DateTime? LastExpenseDate { get; set; }
    public int PendingExpenses { get; set; }
    public int RejectedExpenses { get; set; }
}

public class CategorySummary
{
    public ExpenseCategory Category { get; set; }
    public decimal Total { get; set; }
    public int Count { get; set; }
} 