using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace CRM2.Desktop.Features.Quotations;

public partial class QuotationsView : UserControl
{
    public QuotationsView()
    {
        InitializeComponent();
        this.AttachedToVisualTree += (s, e) => 
        {
            if (DataContext is QuotationsViewModel vm)
            {
                vm.RefreshQuotes();
            }
        };
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
} 