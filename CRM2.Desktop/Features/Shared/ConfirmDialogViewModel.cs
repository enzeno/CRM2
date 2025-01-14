using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CRM2.Desktop.Features.Shared;

public partial class ConfirmDialogViewModel : ObservableObject
{
    [ObservableProperty]
    private string _title;

    [ObservableProperty]
    private string _message;

    public ConfirmDialogViewModel(string title, string message)
    {
        _title = title;
        _message = message;
    }

    [RelayCommand]
    private void Confirm(Window window)
    {
        window.Close(true);
    }

    [RelayCommand]
    private void Cancel(Window window)
    {
        window.Close(false);
    }
} 