using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Controls;
using CRM2.Desktop.Features.Contacts;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CRM2.Desktop.Features.Quotations;

public partial class QuoteDialogViewModel : ObservableObject
{
    private readonly IQuotationService _quotationService;
    private readonly IContactService _contactService;
    private readonly Window _dialog;
    private readonly QuoteDto? _existingQuote;

    [ObservableProperty]
    private string _title;

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    [ObservableProperty]
    private ObservableCollection<ContactDto> _customers = new();

    [ObservableProperty]
    private ContactDto? _selectedCustomer;

    [ObservableProperty]
    private string _currencyCode = "USD";

    [ObservableProperty]
    private ObservableCollection<string> _currencyCodes = new() { "USD", "EUR", "GBP", "JPY" };

    [ObservableProperty]
    private string _customerComments = string.Empty;

    [ObservableProperty]
    private string _internalComments = string.Empty;

    [ObservableProperty]
    private ObservableCollection<QuoteLineItemDto> _lineItems = new();

    [ObservableProperty]
    private QuoteLineItemDto? _selectedLineItem;

    public QuoteDialogViewModel(IQuotationService quotationService, IContactService contactService, Window dialog, QuoteDto? existingQuote = null)
    {
        _quotationService = quotationService;
        _contactService = contactService;
        _dialog = dialog;
        _existingQuote = existingQuote;
        _title = existingQuote == null ? "Create New Quote" : "Edit Quote";

        LoadCustomers();
        if (existingQuote != null)
        {
            LoadExistingQuote(existingQuote);
        }
    }

    private async void LoadCustomers()
    {
        try
        {
            var customers = await _contactService.GetContactsAsync();
            Customers.Clear();
            foreach (var customer in customers)
            {
                Customers.Add(customer);
            }
            StatusMessage = $"Loaded {customers.Count} customers";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error loading customers: {ex.Message}";
        }
    }

    private void LoadExistingQuote(QuoteDto quote)
    {
        SelectedCustomer = Customers.FirstOrDefault(c => c.ContactId == quote.CustomerId);
        CurrencyCode = quote.CurrencyCode;
        CustomerComments = quote.CustomerComments;
        InternalComments = quote.InternalComments;
        
        LineItems.Clear();
        foreach (var item in quote.LineItems)
        {
            LineItems.Add(item);
        }
    }

    [RelayCommand]
    private async Task AddLineItem()
    {
        try
        {
            var lineItemDialog = new LineItemDialog();
            lineItemDialog.DataContext = new LineItemDialogViewModel(lineItemDialog, CurrencyCode);

            var result = await lineItemDialog.ShowDialog<QuoteLineItemDto?>(_dialog);
            if (result != null)
            {
                result.LineNumber = LineItems.Count + 1;
                LineItems.Add(result);
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error adding line item: {ex.Message}";
        }
    }

    [RelayCommand]
    private async Task EditLineItem(QuoteLineItemDto? lineItem)
    {
        if (lineItem == null) return;

        try
        {
            var lineItemDialog = new LineItemDialog();
            lineItemDialog.DataContext = new LineItemDialogViewModel(lineItemDialog, CurrencyCode, lineItem);

            var result = await lineItemDialog.ShowDialog<QuoteLineItemDto?>(_dialog);
            if (result != null)
            {
                var index = LineItems.IndexOf(lineItem);
                result.LineNumber = lineItem.LineNumber;
                LineItems[index] = result;
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error editing line item: {ex.Message}";
        }
    }

    [RelayCommand]
    private void RemoveLineItem(QuoteLineItemDto? lineItem)
    {
        if (lineItem == null) return;

        LineItems.Remove(lineItem);
        // Renumber remaining items
        for (int i = 0; i < LineItems.Count; i++)
        {
            LineItems[i].LineNumber = i + 1;
        }
    }

    [RelayCommand]
    private async Task Save()
    {
        if (SelectedCustomer == null)
        {
            StatusMessage = "Please select a customer";
            return;
        }

        if (!LineItems.Any())
        {
            StatusMessage = "Please add at least one line item";
            return;
        }

        try
        {
            var quote = new QuoteDto
            {
                QuoteId = _existingQuote?.QuoteId ?? string.Empty,
                CustomerId = SelectedCustomer.ContactId,
                CustomerName = SelectedCustomer.OrganizationName,
                CustomerComments = CustomerComments,
                InternalComments = InternalComments,
                CurrencyCode = CurrencyCode,
                Status = "DRAFT",
                LineItems = LineItems.ToList()
            };

            if (_existingQuote == null)
            {
                var createdQuote = await _quotationService.CreateQuoteAsync(quote);
                quote = createdQuote;
            }
            else
            {
                await _quotationService.UpdateQuoteAsync(quote);
            }

            _dialog.Close(quote);
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error saving quote: {ex.Message}";
        }
    }

    [RelayCommand]
    private void Cancel()
    {
        _dialog.Close(null);
    }
} 