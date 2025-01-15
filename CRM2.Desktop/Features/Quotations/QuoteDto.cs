using System;
using System.Collections.Generic;

namespace CRM2.Desktop.Features.Quotations;

public class QuoteDto
{
    public string QuoteId { get; set; } = string.Empty;
    public string CustomerId { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string? CustomerComments { get; set; }
    public string? InternalComments { get; set; }
    public string CurrencyCode { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string CreatedBy { get; set; } = string.Empty;
    public string LastModifiedBy { get; set; } = string.Empty;
    public string? CreatedAt { get; set; }
    public string? UpdatedAt { get; set; }

    public List<QuoteLineItemDto> LineItems { get; set; } = new();
} 