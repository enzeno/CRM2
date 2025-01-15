using System.Collections.Generic;
using System.Threading.Tasks;

namespace CRM2.Desktop.Features.Quotations;

public interface IQuotationService
{
    Task<List<QuoteDto>> GetQuotesAsync();
    Task<QuoteDto> GetQuoteByIdAsync(string id);
    Task<QuoteDto> CreateQuoteAsync(QuoteDto quote);
    Task<QuoteDto> UpdateQuoteAsync(QuoteDto quote);
    Task DeleteQuoteAsync(string id);
} 