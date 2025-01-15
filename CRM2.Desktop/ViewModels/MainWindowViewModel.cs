using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CRM2.Desktop.Features.Contacts;
using CRM2.Desktop.Features.Quotations;
using Microsoft.Extensions.DependencyInjection;
using Avalonia.Controls;

namespace CRM2.Desktop.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    private readonly IServiceProvider _serviceProvider;
    private readonly Window _window;

    [ObservableProperty]
    private object? _currentView;

    public MainWindowViewModel(IServiceProvider serviceProvider, Window window)
    {
        _serviceProvider = serviceProvider;
        _window = window;
        // Initialize with Contacts view
        NavigateToContacts();
    }

    [RelayCommand]
    private void NavigateToContacts()
    {
        var viewModel = ActivatorUtilities.CreateInstance<ContactsViewModel>(_serviceProvider, _window);
        var view = new ContactsView { DataContext = viewModel };
        CurrentView = view;
    }

    [RelayCommand]
    private void NavigateToQuotations()
    {
        var viewModel = ActivatorUtilities.CreateInstance<QuotationsViewModel>(_serviceProvider, _window);
        var view = new QuotationsView { DataContext = viewModel };
        CurrentView = view;
    }

    [RelayCommand]
    private void NavigateToInvoices()
    {
        // TODO: Implement Invoices view
        // CurrentView = new InvoicesViewModel();
    }

    [RelayCommand]
    private void NavigateToReports()
    {
        // TODO: Implement Reports view
        // CurrentView = new ReportsViewModel();
    }
} 