using Microsoft.EntityFrameworkCore;
using cem.Data;
using cem.Models;
using Microsoft.AspNetCore.Components.Forms;
using System.IO;

namespace cem.Services;

public class ExpenseService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ExpenseService> _logger;
    private readonly string _uploadPath;

    public ExpenseService(
        ApplicationDbContext context,
        ILogger<ExpenseService> logger,
        IWebHostEnvironment environment)
    {
        _context = context;
        _logger = logger;
        _uploadPath = Path.Combine(environment.WebRootPath ?? "wwwroot", "uploads");

        // Crea la directory se non esiste
        if (!Directory.Exists(_uploadPath))
        {
            Directory.CreateDirectory(_uploadPath);
        }
    }

    public async Task<List<Expense>> GetExpensesAsync(string userId, bool isAdmin = false)
    {
        var query = _context.Expenses
            .Include(e => e.Condominium)
            .Include(e => e.CreatedBy)
            .Include(e => e.ApprovedBy)
            .Include(e => e.Files)
            .AsQueryable();

        if (!isAdmin)
        {
            query = query.Where(e => e.CreatedById == userId);
        }

        return await query.OrderByDescending(e => e.Date).ToListAsync();
    }

    public async Task<Expense?> GetExpenseAsync(int id, string userId, bool isAdmin = false)
    {
        var query = _context.Expenses
            .Include(e => e.Condominium)
            .Include(e => e.CreatedBy)
            .Include(e => e.ApprovedBy)
            .Include(e => e.Files)
            .Where(e => e.Id == id);

        if (!isAdmin)
        {
            query = query.Where(e => e.CreatedById == userId);
        }

        return await query.FirstOrDefaultAsync();
    }

    public async Task<Expense> CreateExpenseAsync(Expense expense, IBrowserFile? file = null)
    {
        try
        {
            _logger.LogInformation($"Inizio creazione spesa per l'utente {expense.CreatedById}");

            var user = await _context.Users
                .AsNoTracking()
                .Include(u => u.ManagedCondominiums)
                .FirstOrDefaultAsync(u => u.Id == expense.CreatedById);

            if (user == null)
            {
                _logger.LogWarning($"Utente {expense.CreatedById} non trovato");
                throw new ArgumentException("Utente non trovato");
            }

            _logger.LogInformation($"Utente trovato: {user.UserName}");
            _logger.LogInformation($"Verifica accesso al condominio {expense.CondominiumId}");

            if (!user.ManagedCondominiums.Any(c => c.Id == expense.CondominiumId))
            {
                _logger.LogWarning($"L'utente {user.UserName} non ha accesso al condominio {expense.CondominiumId}");
                throw new ArgumentException("L'utente non ha accesso a questo condominio");
            }

            expense.CreatedAt = DateTime.UtcNow;
            expense.Status = ExpenseStatus.Pending;

            _logger.LogInformation($"Aggiunta spesa al contesto: {expense.Description}");
            _context.Expenses.Add(expense);
            await _context.SaveChangesAsync();

            if (file != null)
            {
                await SaveFileAsync(expense, file, user);
            }

            _logger.LogInformation($"Spesa creata con successo: {expense.Id}");
            return expense;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Errore durante la creazione della spesa per l'utente {expense.CreatedById}");
            throw;
        }
    }

    public async Task<Expense> UpdateExpenseAsync(Expense expense, IBrowserFile? file = null)
    {
        var user = await _context.Users.FindAsync(expense.CreatedById);
        if (user == null)
        {
            throw new ArgumentException("Utente non trovato");
        }

        _context.Entry(expense).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        if (file != null)
        {
            foreach (var existingFile in expense.Files)
            {
                var filePath = Path.Combine(_uploadPath, existingFile.FilePath);
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                _context.Files.Remove(existingFile);
            }
            await _context.SaveChangesAsync();

            await SaveFileAsync(expense, file, user);
        }

        return expense;
    }

    private async Task SaveFileAsync(Expense expense, IBrowserFile file, ApplicationUser user)
    {
        try
        {
            var fileName = $"{Guid.NewGuid()}_{file.Name}";
            var filePath = Path.Combine(_uploadPath, fileName);

            using (var stream = File.Create(filePath))
            {
                await file.OpenReadStream(10 * 1024 * 1024).CopyToAsync(stream);
            }

            var appFile = new AppFile
            {
                FileName = file.Name,
                ContentType = file.ContentType,
                FilePath = fileName,
                FileSize = file.Size,
                UploadedById = user.Id,
                EntityType = "Expense",
                EntityId = expense.Id
            };

            _context.Files.Add(appFile);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"File salvato con successo: {fileName}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Errore durante il salvataggio del file");
            throw;
        }
    }

    public async Task<bool> DeleteExpenseAsync(int id, string userId, bool isAdmin = false)
    {
        var expense = await GetExpenseAsync(id, userId, isAdmin);
        if (expense == null)
        {
            return false;
        }

        foreach (var file in expense.Files)
        {
            var filePath = Path.Combine(_uploadPath, file.FilePath);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        _context.Expenses.Remove(expense);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<Expense> ApproveExpenseAsync(int id, string approverId)
    {
        var expense = await _context.Expenses.FindAsync(id);
        if (expense == null)
        {
            throw new ArgumentException("Spesa non trovata");
        }

        expense.Status = ExpenseStatus.Approved;
        expense.ApprovedById = approverId;
        expense.ApprovedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return expense;
    }

    public async Task<Expense> RejectExpenseAsync(int id, string approverId)
    {
        var expense = await _context.Expenses.FindAsync(id);
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
}