using System;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Avalonia.Controls;
using CRM2.Desktop.Features.Contacts;
using CRM2.Desktop.Features.Quotations;
using CRM2.Desktop.Services;

namespace CRM2.Desktop.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    private readonly IQuotationService _quotationService;
    private readonly IContactService _contactService;
    private readonly IPdfExportService _pdfExportService;
    private readonly Window _parentWindow;
    private readonly IServiceProvider _serviceProvider;

    [ObservableProperty]
    private Control? _currentView;

    public MainWindowViewModel(
        IQuotationService quotationService,
        IContactService contactService,
        IPdfExportService pdfExportService,
        Window parentWindow,
        IServiceProvider serviceProvider)
    {
        _quotationService = quotationService;
        _contactService = contactService;
        _pdfExportService = pdfExportService;
        _parentWindow = parentWindow;
        _serviceProvider = serviceProvider;

        // Set initial view
        NavigateToQuotations();
    }

    [RelayCommand]
    private void NavigateToContacts()
    {
        var contactsView = new ContactsView
        {
            DataContext = new ContactsViewModel(_contactService, _parentWindow)
        };
        CurrentView = contactsView;
    }

    [RelayCommand]
    private void NavigateToQuotations()
    {
        var quotationsView = new QuotationsView
        {
            DataContext = new QuotationsViewModel(_quotationService, _pdfExportService, _parentWindow, _serviceProvider)
        };
        CurrentView = quotationsView;
    }

    [RelayCommand]
    private void NavigateToInvoices()
    {
        // To be implemented
    }

    [RelayCommand]
    private void NavigateToReports()
    {
        // To be implemented
    }
} 