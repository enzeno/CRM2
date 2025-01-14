using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System.Threading.Tasks;

namespace CRM2.Desktop.Features.Shared;

public partial class MessageBox : Window
{
    private string _title = string.Empty;
    public string DialogTitle 
    { 
        get => _title;
        set
        {
            _title = value;
            Title = value;
        }
    }
    public string Message { get; }
    public bool ShowCancel { get; }

    public MessageBox()
    {
        DialogTitle = string.Empty;
        Message = string.Empty;
        ShowCancel = false;
        InitializeComponent();
    }

    public MessageBox(string title, string message, bool showCancel = false)
    {
        DialogTitle = title;
        Message = message;
        ShowCancel = showCancel;
        DataContext = this;
        
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void OkButton_Click(object sender, RoutedEventArgs e)
    {
        Close(true);
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        Close(false);
    }

    public static async Task<bool> ShowDialog(Window owner, string title, string message, bool showCancel = false)
    {
        var dialog = new MessageBox(title, message, showCancel);
        var result = await dialog.ShowDialog<bool>(owner);
        return result;
    }
} 