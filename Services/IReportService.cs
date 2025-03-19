using cem.Models;

namespace cem.Services;

public interface IReportService
{
    Task<byte[]> GenerateExpensesReportAsync(int condominiumId, DateTime? startDate = null, DateTime? endDate = null);
} 