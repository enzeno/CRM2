using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using CRM2.Desktop.Services;
using System.Threading.Tasks;

namespace CRM2.Desktop.Features.Contacts;

public partial class ContactDialog : Window
{
    public ContactDialog()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public static async Task<Window> ShowDialog(Window owner, ContactService contactService, ContactDto? contact = null)
    {
        var dialog = new ContactDialog
        {
            DataContext = new ContactDialogViewModel(owner, contactService, contact)
        };

        await dialog.ShowDialog(owner);
        return dialog;
    }
} 