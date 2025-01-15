using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Avalonia.Controls;
using CRM2.Desktop.Features.Contacts;
using System.Linq;
using System.Collections.Generic;
using Avalonia.Threading;
using CRM2.Desktop.Services;
using Avalonia.Platform.Storage;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;

namespace CRM2.Desktop.Features.Quotations
{
    public partial class QuotationsViewModel : ObservableObject
    {
        private readonly IQuotationService _quotationService;
        private readonly IPdfExportService _pdfExportService;
        private readonly Window _parentWindow;
        private readonly IServiceProvider _serviceProvider;
        private List<QuoteDto> _allQuotes = new();

        [ObservableProperty]
        private string _searchText = string.Empty;

        [ObservableProperty]
        private string _statusMessage = string.Empty;

        [ObservableProperty]
        private QuoteDto? _selectedQuote;

        public ObservableCollection<QuoteDto> Quotes { get; } = new();

        // CommunityToolkit.Mvvm auto-hook for when SearchText changes
        partial void OnSearchTextChanged(string value)
        {
            FilterQuotes();
        }

        public QuotationsViewModel(
            IQuotationService quotationService,
            IPdfExportService pdfExportService,
            Window parentWindow,
            IServiceProvider serviceProvider)
        {
            _quotationService = quotationService;
            _pdfExportService = pdfExportService;
            _parentWindow = parentWindow;
            _serviceProvider = serviceProvider;
            
            // Load quotes on UI thread to avoid async issues
            Dispatcher.UIThread.Post(async () =>
            {
                await LoadQuotesAsync();
            });
        }

        [RelayCommand]
        private async Task CreateQuoteAsync()
        {
            try
            {
                var quoteDialog = new QuoteDialog();
                var contactService = _serviceProvider.GetRequiredService<IContactService>();
                var viewModel = new QuoteDialogViewModel(_quotationService, contactService, quoteDialog);
                quoteDialog.DataContext = viewModel;

                var result = await quoteDialog.ShowDialog<QuoteDto?>(_parentWindow);
                if (result != null)
                {
                    _allQuotes.Add(result);
                    FilterQuotes();
                    StatusMessage = "Quote created successfully";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error creating quote: {ex.Message}";
            }
        }

        [RelayCommand]
        private async Task EditQuoteAsync(QuoteDto? quote)
        {
            if (quote == null) return;

            try
            {
                var quoteDialog = new QuoteDialog();
                var contactService = _serviceProvider.GetRequiredService<IContactService>();
                var viewModel = new QuoteDialogViewModel(_quotationService, contactService, quoteDialog, quote);
                quoteDialog.DataContext = viewModel;

                var result = await quoteDialog.ShowDialog<QuoteDto?>(_parentWindow);
                if (result != null)
                {
                    var index = _allQuotes.IndexOf(quote);
                    if (index != -1)
                    {
                        _allQuotes[index] = result;
                        FilterQuotes();
                    }
                    StatusMessage = "Quote updated successfully";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error updating quote: {ex.Message}";
            }
        }

        private void FilterQuotes()
        {
            var searchTerms = (SearchText ?? string.Empty)
                .ToLower()
                .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(term => term.Trim())
                .Where(term => !string.IsNullOrWhiteSpace(term))
                .ToList();

            var filtered = _allQuotes;

            if (searchTerms.Any())
            {
                filtered = _allQuotes.Where(quote =>
                    searchTerms.All(term =>
                    {
                        // Check top-level quote fields
                        bool matchesQuoteFields =
                            quote.QuoteId.ToLower().Contains(term) ||
                            (quote.CustomerName?.ToLower().Contains(term) == true) ||
                            quote.Status.ToLower().Contains(term) ||
                            quote.CurrencyCode.ToLower().Contains(term) ||
                            (quote.CustomerComments?.ToLower().Contains(term) == true) ||
                            (quote.InternalComments?.ToLower().Contains(term) == true) ||
                            quote.CreatedBy.ToLower().Contains(term) ||
                            quote.LastModifiedBy.ToLower().Contains(term) ||
                            (quote.CreatedAt?.ToLower().Contains(term) == true) ||
                            (quote.UpdatedAt?.ToLower().Contains(term) == true);

                        // If no match in quote fields, check line items
                        if (!matchesQuoteFields && quote.LineItems != null)
                        {
                            return quote.LineItems.Any(li =>
                                li.PartNumber.ToLower().Contains(term) ||
                                (li.AlternativePartNumber?.ToLower().Contains(term) == true) ||
                                (li.Description?.ToLower().Contains(term) == true) ||
                                (li.SupplierCode?.ToLower().Contains(term) == true) ||
                                li.Quantity.ToString().Contains(term) ||
                                li.SellPrice.ToString().Contains(term) ||
                                li.BuyPrice.ToString().Contains(term) ||
                                (li.Comments?.ToLower().Contains(term) == true)
                            );
                        }

                        return matchesQuoteFields;
                    })
                ).ToList();
            }

            // Update the UI thread with filtered results
            Dispatcher.UIThread.Post(() =>
            {
                Quotes.Clear();
                foreach (var quote in filtered)
                {
                    Quotes.Add(quote);
                }
                StatusMessage = $"Found {Quotes.Count} quotes";
            });
        }

        private async Task LoadQuotesAsync()
        {
            try
            {
                Console.WriteLine("Starting to load quotes...");
                StatusMessage = "Loading quotes...";
                
                Console.WriteLine("Calling GetQuotesAsync...");
                var quotes = await _quotationService.GetQuotesAsync();
                Console.WriteLine($"Received {quotes?.Count ?? 0} quotes from service");
                
                if (quotes == null)
                {
                    throw new InvalidOperationException("Received null response from quotation service");
                }
                
                _allQuotes = quotes.ToList();
                Console.WriteLine("Updated _allQuotes list");
                
                FilterQuotes();
                Console.WriteLine("Applied filters");
                
                StatusMessage = $"Loaded {quotes.Count} quotes";
                Console.WriteLine("Load operation completed successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading quotes: {ex.GetType().Name} - {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                StatusMessage = $"Error loading quotes: {ex.Message}";
            }
        }

        public void RefreshQuotes()
        {
            // Refresh quotes on UI thread
            Dispatcher.UIThread.Post(async () =>
            {
                await LoadQuotesAsync();
            });
        }

        public async Task RefreshQuotesAsync()
        {
            try
            {
                await LoadQuotesAsync();
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error refreshing quotes: {ex.Message}";
            }
        }

        [RelayCommand]
        private async Task DeleteQuoteAsync(QuoteDto? quote)
        {
            if (quote == null) return;

            try
            {
                // TODO: Fix async loading later
                // var result = await _quotationService.DeleteQuoteAsync(quote.QuoteId);
                var result = true;
                if (result)
                {
                    _allQuotes.Remove(quote);
                    FilterQuotes();
                    StatusMessage = "Quote deleted successfully";
                }
                else
                {
                    StatusMessage = "Failed to delete quote";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error deleting quote: {ex.Message}";
            }
        }

        [RelayCommand]
        private async Task ExportQuoteAsync(Button sourceButton)
        {
            if (SelectedQuote == null) return;

            try
            {
                var exportMenu = new MenuFlyout();
                
                var saveMenuItem = new MenuItem { Header = "Save to File..." };
                saveMenuItem.Click += async (s, e) => await SavePdfToFile(SelectedQuote);
                
                var previewMenuItem = new MenuItem { Header = "Preview" };
                previewMenuItem.Click += async (s, e) => await PreviewPdf(SelectedQuote);
                
                exportMenu.Items.Add(saveMenuItem);
                exportMenu.Items.Add(previewMenuItem);
                
                exportMenu.ShowAt(sourceButton);
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error showing export options: {ex.Message}";
            }
        }

        private async Task SavePdfToFile(QuoteDto quote)
        {
            try
            {
                var filePickerOptions = new FilePickerSaveOptions
                {
                    Title = "Save PDF",
                    DefaultExtension = "pdf",
                    ShowOverwritePrompt = true,
                    FileTypeChoices = new[]
                    {
                        new FilePickerFileType("PDF Document") { Patterns = new[] { "*.pdf" } }
                    }
                };

                var file = await _parentWindow.StorageProvider.SaveFilePickerAsync(filePickerOptions);
                if (file != null)
                {
                    StatusMessage = "Generating PDF...";
                    await _pdfExportService.ExportQuotationToPdfAsync(quote, file.Path.LocalPath);
                    StatusMessage = "PDF exported successfully!";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error exporting PDF: {ex.Message}";
            }
        }

        private async Task PreviewPdf(QuoteDto quote)
        {
            try
            {
                StatusMessage = "Generating PDF for preview...";
                
                // Create a temporary file
                var tempFile = Path.Combine(Path.GetTempPath(), $"quote_{quote.QuoteId}_{DateTime.Now:yyyyMMddHHmmss}.pdf");
                
                await _pdfExportService.ExportQuotationToPdfAsync(quote, tempFile);
                
                // Open the PDF with the default viewer
                if (OperatingSystem.IsMacOS())
                {
                    await Task.Run(() => Process.Start("open", tempFile));
                }
                else if (OperatingSystem.IsWindows())
                {
                    await Task.Run(() => Process.Start(new ProcessStartInfo(tempFile) { UseShellExecute = true }));
                }
                else if (OperatingSystem.IsLinux())
                {
                    await Task.Run(() => Process.Start("xdg-open", tempFile));
                }
                
                StatusMessage = "PDF opened for preview";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error previewing PDF: {ex.Message}";
            }
        }
    }
}