using System;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace CRM2.Desktop.Features.Contacts;

public partial class ContactsView : UserControl
{
    public ContactsView()
    {
        InitializeComponent();
        AttachedToVisualTree += (s, e) =>
        {
            var window = TopLevel.GetTopLevel(this) as Window ?? throw new InvalidOperationException("Window not found");
            DataContext = new ContactsViewModel(window);
        };
    }
} 