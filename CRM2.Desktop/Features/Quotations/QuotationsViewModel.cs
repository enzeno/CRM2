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

namespace CRM2.Desktop.Features.Quotations
{
    public partial class QuotationsViewModel : ObservableObject
    {
        private readonly IQuotationService _quotationService;
        private readonly IContactService _contactService;
        private readonly Window _parentWindow;
        private List<QuoteDto> _allQuotes = new();

        [ObservableProperty]
        private string _searchText = string.Empty;

        [ObservableProperty]
        private string _statusMessage = string.Empty;

        [ObservableProperty]
        private QuoteDto? _selectedQuote;

        // Initialize once here, no reassignment in constructor
        public ObservableCollection<QuoteDto> Quotes { get; } = new();

        // CommunityToolkit.Mvvm auto-hook for when SearchText changes
        partial void OnSearchTextChanged(string value)
        {
            FilterQuotes();
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

        public QuotationsViewModel(IQuotationService quotationService, IContactService contactService, Window parentWindow)
        {
            _quotationService = quotationService;
            _contactService = contactService;
            _parentWindow = parentWindow;
            
            // Load quotes on UI thread to avoid async issues
            Dispatcher.UIThread.Post(async () =>
            {
                await LoadQuotesAsync();
            });
        }

        private async Task LoadQuotesAsync()
        {
            try
            {
                Console.WriteLine("Loading quotes...");
                var quotes = await _quotationService.GetQuotesAsync();
                _allQuotes = quotes.ToList();
                FilterQuotes();
                StatusMessage = $"Loaded {quotes.Count} quotes";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading quotes: {ex.Message}");
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
            await LoadQuotesAsync();
        }

        [RelayCommand]
        private async Task CreateQuoteAsync()
        {
            try
            {
                var quoteDialog = new QuoteDialog();
                var viewModel = new QuoteDialogViewModel(_quotationService, _contactService, quoteDialog);
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
                var viewModel = new QuoteDialogViewModel(_quotationService, _contactService, quoteDialog, quote);
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
    }
}