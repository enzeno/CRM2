using System.Threading.Tasks;
using CRM2.Desktop.Features.Quotations;

namespace CRM2.Desktop.Services;

public interface IPdfExportService
{
    Task ExportQuotationToPdfAsync(QuoteDto quote, string outputPath);
} 