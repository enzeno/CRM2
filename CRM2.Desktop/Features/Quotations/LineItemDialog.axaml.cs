using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace CRM2.Desktop.Features.Quotations;

public partial class LineItemDialog : Window
{
    public LineItemDialog()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
} 