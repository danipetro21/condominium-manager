using iTextSharp.text;
using iTextSharp.text.pdf;
using cem.Models;
using Microsoft.EntityFrameworkCore;
using cem.Data;

namespace cem.Services;

public class ReportService : IReportService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ReportService> _logger;

    public ReportService(
        ApplicationDbContext context,
        ILogger<ReportService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<byte[]> GenerateExpensesReportAsync(int condominiumId, DateTime? startDate = null, DateTime? endDate = null)
    {
        try
        {
            using var memoryStream = new MemoryStream();
            var document = new Document(PageSize.A4, 50, 50, 50, 50);
            var writer = PdfWriter.GetInstance(document, memoryStream);
            document.Open();

            // Aggiungi intestazione
            var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16);
            var title = new Paragraph("Report Spese Condominio", titleFont);
            title.Alignment = Element.ALIGN_CENTER;
            title.SpacingAfter = 20f;
            document.Add(title);

            var condominium = await _context.Condominiums.FindAsync(condominiumId);
            if (condominium == null)
            {
                throw new ArgumentException("Condominio non trovato");
            }

            var condominiumInfo = new Paragraph();
            condominiumInfo.Add(new Chunk("Nome: ", FontFactory.GetFont(FontFactory.HELVETICA_BOLD)));
            condominiumInfo.Add(new Chunk(condominium.Name + "\n", FontFactory.GetFont(FontFactory.HELVETICA)));
            condominiumInfo.Add(new Chunk("Indirizzo: ", FontFactory.GetFont(FontFactory.HELVETICA_BOLD)));
            condominiumInfo.Add(new Chunk($"{condominium.Address}, {condominium.City} ({condominium.Province})\n", FontFactory.GetFont(FontFactory.HELVETICA)));
            condominiumInfo.Add(new Chunk("CAP: ", FontFactory.GetFont(FontFactory.HELVETICA_BOLD)));
            condominiumInfo.Add(new Chunk(condominium.PostalCode + "\n", FontFactory.GetFont(FontFactory.HELVETICA)));
            condominiumInfo.SpacingAfter = 20f;
            document.Add(condominiumInfo);

            var query = _context.Expenses
                .Include(e => e.CreatedBy)
                .Where(e => e.CondominiumId == condominiumId);

            if (startDate.HasValue)
            {
                query = query.Where(e => e.Date >= startDate.Value);
            }
            if (endDate.HasValue)
            {
                query = query.Where(e => e.Date <= endDate.Value);
            }

            var expenses = await query.OrderByDescending(e => e.Date).ToListAsync();

            var summaryFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);
            var summary = new Paragraph("Riepilogo Spese", summaryFont);
            summary.SpacingAfter = 10f;
            document.Add(summary);

            var totalApproved = expenses.Where(e => e.Status == ExpenseStatus.Approved).Sum(e => e.Amount);
            var totalPending = expenses.Where(e => e.Status == ExpenseStatus.Pending).Sum(e => e.Amount);
            var totalRejected = expenses.Where(e => e.Status == ExpenseStatus.Rejected).Sum(e => e.Amount);

            var summaryTable = new PdfPTable(2);
            summaryTable.WidthPercentage = 100;
            summaryTable.AddCell(new PdfPCell(new Phrase("Totale Spese Approvate:", FontFactory.GetFont(FontFactory.HELVETICA_BOLD))));
            summaryTable.AddCell(new PdfPCell(new Phrase(totalApproved.ToString("C"))));
            summaryTable.AddCell(new PdfPCell(new Phrase("Totale Spese in Attesa:", FontFactory.GetFont(FontFactory.HELVETICA_BOLD))));
            summaryTable.AddCell(new PdfPCell(new Phrase(totalPending.ToString("C"))));
            summaryTable.AddCell(new PdfPCell(new Phrase("Totale Spese Rifiutate:", FontFactory.GetFont(FontFactory.HELVETICA_BOLD))));
            summaryTable.AddCell(new PdfPCell(new Phrase(totalRejected.ToString("C"))));
            summaryTable.SpacingAfter = 20f;
            document.Add(summaryTable);

            var expensesFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);
            var expensesTitle = new Paragraph("Dettaglio Spese", expensesFont);
            expensesTitle.SpacingAfter = 10f;
            document.Add(expensesTitle);

            var table = new PdfPTable(6);
            table.WidthPercentage = 100;
            table.SetWidths(new float[] { 1f, 2f, 1f, 1f, 1f, 1f });

            table.AddCell(new PdfPCell(new Phrase("Data", FontFactory.GetFont(FontFactory.HELVETICA_BOLD))));
            table.AddCell(new PdfPCell(new Phrase("Descrizione", FontFactory.GetFont(FontFactory.HELVETICA_BOLD))));
            table.AddCell(new PdfPCell(new Phrase("Categoria", FontFactory.GetFont(FontFactory.HELVETICA_BOLD))));
            table.AddCell(new PdfPCell(new Phrase("Importo", FontFactory.GetFont(FontFactory.HELVETICA_BOLD))));
            table.AddCell(new PdfPCell(new Phrase("Stato", FontFactory.GetFont(FontFactory.HELVETICA_BOLD))));
            table.AddCell(new PdfPCell(new Phrase("Creato da", FontFactory.GetFont(FontFactory.HELVETICA_BOLD))));

            // Dati
            foreach (var expense in expenses)
            {
                table.AddCell(new PdfPCell(new Phrase(expense.Date.ToShortDateString())));
                table.AddCell(new PdfPCell(new Phrase(expense.Description)));
                table.AddCell(new PdfPCell(new Phrase(expense.Category.ToString())));
                table.AddCell(new PdfPCell(new Phrase(expense.Amount.ToString("C"))));
                table.AddCell(new PdfPCell(new Phrase(expense.Status.ToString())));
                table.AddCell(new PdfPCell(new Phrase(expense.CreatedBy.Email)));
            }

            document.Add(table);

            var footer = new Paragraph($"Report generato il {DateTime.Now:dd/MM/yyyy HH:mm}", FontFactory.GetFont(FontFactory.HELVETICA, 8));
            footer.Alignment = Element.ALIGN_RIGHT;
            footer.SpacingBefore = 20f;
            document.Add(footer);

            document.Close();
            return memoryStream.ToArray();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Errore durante la generazione del report PDF");
            throw;
        }
    }
}