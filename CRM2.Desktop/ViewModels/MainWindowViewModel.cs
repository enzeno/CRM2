using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CRM2.Desktop.Features.Contacts;

namespace CRM2.Desktop.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    [ObservableProperty]
    private object? _currentView;

    public MainWindowViewModel()
    {
        // Initialize with Contacts view
        NavigateToContacts();
    }

    [RelayCommand]
    private void NavigateToContacts()
    {
        CurrentView = new ContactsView();
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