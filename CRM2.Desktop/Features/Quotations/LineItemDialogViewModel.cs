using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Avalonia.Controls;

namespace CRM2.Desktop.Features.Quotations;

public partial class LineItemDialogViewModel : ObservableObject
{
    private readonly Window _dialog;

    [ObservableProperty]
    private string _title;

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    [ObservableProperty]
    private string _partNumber = string.Empty;

    [ObservableProperty]
    private string? _alternativePartNumber;

    [ObservableProperty]
    private string _description = string.Empty;

    [ObservableProperty]
    private decimal _quantity = 1;

    [ObservableProperty]
    private decimal _sellPrice;

    [ObservableProperty]
    private decimal _buyPrice;

    [ObservableProperty]
    private string _currencyCode;

    [ObservableProperty]
    private ObservableCollection<string> _currencyCodes = new() { "USD", "EUR", "GBP", "JPY" };

    [ObservableProperty]
    private string? _supplierCode;

    [ObservableProperty]
    private string? _comments;

    public LineItemDialogViewModel(Window dialog, string defaultCurrency, QuoteLineItemDto? existingItem = null)
    {
        _dialog = dialog;
        _currencyCode = defaultCurrency;
        _title = existingItem == null ? "Add Line Item" : "Edit Line Item";

        if (existingItem != null)
        {
            LoadExistingItem(existingItem);
        }
    }

    private void LoadExistingItem(QuoteLineItemDto item)
    {
        PartNumber = item.PartNumber;
        AlternativePartNumber = item.AlternativePartNumber;
        Description = item.Description;
        Quantity = item.Quantity;
        SellPrice = item.SellPrice;
        BuyPrice = item.BuyPrice;
        CurrencyCode = item.CurrencyCode;
        SupplierCode = item.SupplierCode;
        Comments = item.Comments;
    }

    [RelayCommand]
    private void Save()
    {
        if (string.IsNullOrWhiteSpace(PartNumber))
        {
            StatusMessage = "Part number is required";
            return;
        }

        if (string.IsNullOrWhiteSpace(Description))
        {
            StatusMessage = "Description is required";
            return;
        }

        if (Quantity <= 0)
        {
            StatusMessage = "Quantity must be greater than 0";
            return;
        }

        if (SellPrice < 0)
        {
            StatusMessage = "Sell price cannot be negative";
            return;
        }

        if (BuyPrice < 0)
        {
            StatusMessage = "Buy price cannot be negative";
            return;
        }

        var lineItem = new QuoteLineItemDto
        {
            PartNumber = PartNumber,
            AlternativePartNumber = AlternativePartNumber,
            Description = Description,
            Quantity = Quantity,
            SellPrice = SellPrice,
            BuyPrice = BuyPrice,
            CurrencyCode = CurrencyCode,
            SupplierCode = SupplierCode,
            Comments = Comments
        };

        _dialog.Close(lineItem);
    }

    [RelayCommand]
    private void Cancel()
    {
        _dialog.Close(null);
    }
} 