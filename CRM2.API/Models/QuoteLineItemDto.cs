namespace CRM2.API.Models;

public class QuoteLineItemDto
{
    public string QuoteId { get; set; } = string.Empty;
    public int LineNumber { get; set; }
    public string PartNumber { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? AlternativePartNumber { get; set; }
    public string? SupplierCode { get; set; }
    public decimal Quantity { get; set; }
    public string UnitOfMeasure { get; set; } = string.Empty;
    public decimal BuyPrice { get; set; }
    public decimal SellPrice { get; set; }
    public string CurrencyCode { get; set; } = string.Empty;
    public string? Comments { get; set; }

    public decimal Total => Quantity * SellPrice;
} 