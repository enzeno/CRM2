using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace CRM2.Desktop.Features.Contacts;

public partial class ContactDialog : Window
{
    private readonly IContactService _contactService;
    private readonly Window _parentWindow;

    public ContactDialog(IContactService contactService, Window parentWindow)
    {
        _contactService = contactService;
        _parentWindow = parentWindow;
        InitializeComponent();
        DataContext = new ContactDialogViewModel(_contactService, this);
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
} 