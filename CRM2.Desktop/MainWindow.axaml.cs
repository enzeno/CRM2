using Avalonia.Controls;
using CRM2.Desktop.ViewModels;

namespace CRM2.Desktop;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainWindowViewModel();
    }
}