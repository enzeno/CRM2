using Avalonia.Controls;
using System.Threading.Tasks;

namespace CRM2.Desktop.Features.Shared;

public partial class ConfirmDialog : Window
{
    public ConfirmDialog()
    {
        InitializeComponent();
    }

    public static async Task<bool> ShowDialog(Window owner, string title, string message)
    {
        var dialog = new ConfirmDialog
        {
            DataContext = new ConfirmDialogViewModel(title, message)
        };

        var result = await dialog.ShowDialog<bool>(owner);
        return result;
    }
} 