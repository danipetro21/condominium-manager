using Microsoft.EntityFrameworkCore;
using cem.Data;
using cem.Models;

namespace cem.Services;

public class CondominiumService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<CondominiumService> _logger;

    public CondominiumService(
        ApplicationDbContext context,
        ILogger<CondominiumService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<Condominium>> GetUserCondominiumsAsync(string userId)
    {
        try
        {
            var query = _context.UserCondominiums
                .AsNoTracking()
                .Include(uc => uc.ManagedCondominium)
                .Where(uc => uc.ManagersId == userId)
                .Select(uc => uc.ManagedCondominium);

            return await query.ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Errore durante il caricamento dei condomini per l'utente {userId}");
            throw;
        }
    }

    public async Task<Condominium?> GetCondominiumAsync(int id, string userId)
    {
        try
        {
            var query = _context.UserCondominiums
                .AsNoTracking()
                .Include(uc => uc.ManagedCondominium)
                .Where(uc => uc.ManagersId == userId && uc.ManagedCondominiumsId == id)
                .Select(uc => uc.ManagedCondominium);

            return await query.FirstOrDefaultAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Errore durante il caricamento del condominio {id} per l'utente {userId}");
            throw;
        }
    }

    public async Task<CondominiumSummary> GetCondominiumSummaryAsync(int id, string userId)
    {
        try
        {
            var condominium = await GetCondominiumAsync(id, userId);
            if (condominium == null)
            {
                throw new ArgumentException("Condominio non trovato");
            }

            var expenses = await _context.Expenses
                .AsNoTracking()
                .Where(e => e.CondominiumId == id && e.Status == ExpenseStatus.Approved)
                .ToListAsync();

            var totalExpenses = expenses.Sum(e => e.Amount);
            var expensesByCategory = expenses
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
                LastExpenseDate = expenses.Any() ? expenses.Max(e => e.Date) : null
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Errore durante il calcolo del riepilogo del condominio {id}");
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
}

public class CategorySummary
{
    public ExpenseCategory Category { get; set; }
    public decimal Total { get; set; }
    public int Count { get; set; }
} 