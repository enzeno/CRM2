using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using CRM2.Desktop.Features.Quotations;

namespace CRM2.Desktop.Services;

public class PdfExportService : IPdfExportService
{
    public PdfExportService()
    {
        try
        {
            Console.WriteLine("Initializing QuestPDF...");
            QuestPDF.Settings.License = LicenseType.Community;
            Console.WriteLine("QuestPDF license set to Community");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error initializing QuestPDF: {ex.GetType().Name} - {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            throw;
        }
    }

    public async Task ExportQuotationToPdfAsync(QuoteDto quote, string outputPath)
    {
        try
        {
            Console.WriteLine($"Starting PDF export for quote {quote.QuoteId} to {outputPath}");
            Console.WriteLine("Creating PDF document...");

            Document.Create(container =>
            {
                try
                {
                    container.Page(page =>
                    {
                        Console.WriteLine("Configuring page settings...");
                        page.Size(PageSizes.A4);
                        page.Margin(2, Unit.Centimetre);
                        page.DefaultTextStyle(x => x.FontSize(10));

                        Console.WriteLine("Composing header...");
                        page.Header().Element(ComposeHeader);
                        
                        Console.WriteLine("Composing content...");
                        page.Content().Element(x => ComposeContent(x, quote));
                        
                        Console.WriteLine("Composing footer...");
                        page.Footer().Element(ComposeFooter);
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in document composition: {ex.GetType().Name} - {ex.Message}");
                    Console.WriteLine($"Stack trace: {ex.StackTrace}");
                    throw;
                }
            })
            .GeneratePdf(outputPath);

            Console.WriteLine("PDF generation completed successfully");
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error exporting PDF: {ex.GetType().Name} - {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            throw;
        }
    }

    private void ComposeHeader(IContainer container)
    {
        try
        {
            container.Row(row =>
            {
                row.RelativeItem().Column(column =>
                {
                    column.Item().Text("Your Company Name")
                        .FontSize(20)
                        .SemiBold();
                    column.Item().Text("123 Business Street");
                    column.Item().Text("City, Country");
                    column.Item().Text("Phone: +1234567890");
                });

                row.RelativeItem().Column(column =>
                {
                    column.Item().Text("QUOTATION")
                        .FontSize(24)
                        .SemiBold()
                        .FontColor(Colors.Blue.Medium);
                });
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error composing header: {ex.GetType().Name} - {ex.Message}");
            throw;
        }
    }

    private void ComposeContent(IContainer container, QuoteDto quote)
    {
        try
        {
            Console.WriteLine("Composing quote details...");
            container.Column(column =>
            {
                column.Spacing(20);

                column.Item().Row(row =>
                {
                    row.RelativeItem().Column(column =>
                    {
                        column.Item().Text("Bill To:").SemiBold();
                        column.Item().Text(quote.CustomerName);
                        column.Item().Text($"Customer ID: {quote.CustomerId}");
                    });

                    row.RelativeItem().Column(column =>
                    {
                        column.Item().Text("Quote Details:").SemiBold();
                        column.Item().Text($"Quote #: {quote.QuoteId}");
                        column.Item().Text($"Date: {quote.CreatedAt}");
                        column.Item().Text($"Status: {quote.Status}");
                    });
                });

                if (!string.IsNullOrEmpty(quote.CustomerComments))
                {
                    column.Item().Column(column =>
                    {
                        column.Item().Text("Customer Comments:").SemiBold();
                        column.Item().Text(quote.CustomerComments);
                    });
                }

                Console.WriteLine("Composing line items table...");
                column.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(3);
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                    });

                    table.Header(header =>
                    {
                        header.Cell().Text("Description").SemiBold();
                        header.Cell().Text("Quantity").SemiBold();
                        header.Cell().Text("Unit Price").SemiBold();
                        header.Cell().Text("Total").SemiBold();
                    });

                    var lineItems = quote.LineItems?.ToList() ?? new List<QuoteLineItemDto>();
                    Console.WriteLine($"Processing {lineItems.Count} line items");
                    
                    foreach (var item in lineItems)
                    {
                        table.Cell().Element(cell => cell.Text(item.Description));
                        table.Cell().Element(cell => cell.Text(item.Quantity.ToString()));
                        table.Cell().Element(cell => cell.Text($"{item.SellPrice:C}"));
                        table.Cell().Element(cell => cell.Text($"{item.Quantity * item.SellPrice:C}"));
                    }

                    var total = lineItems.Sum(x => x.Quantity * x.SellPrice);
                    table.Cell().ColumnSpan(3).Element(cell => cell.Text("Total:").SemiBold());
                    table.Cell().Element(cell => cell.Text($"{total:C}").SemiBold());
                });
            });
            Console.WriteLine("Content composition completed");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error composing content: {ex.GetType().Name} - {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            throw;
        }
    }

    private void ComposeFooter(IContainer container)
    {
        try
        {
            container.Row(row =>
            {
                row.RelativeItem().Column(column =>
                {
                    column.Item().Text("Terms & Conditions").SemiBold();
                    column.Item().Text("1. Valid for 30 days");
                    column.Item().Text("2. Subject to our terms of service");
                });
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error composing footer: {ex.GetType().Name} - {ex.Message}");
            throw;
        }
    }
} 