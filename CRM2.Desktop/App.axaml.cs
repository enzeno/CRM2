using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using CRM2.Desktop.ViewModels;

namespace CRM2.Desktop;

public partial class App : Application
{
    private ServiceProvider? _serviceProvider;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);

        var services = new ServiceCollection();
        services.AddServices();
        _serviceProvider = services.BuildServiceProvider();
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var mainWindow = new MainWindow();
            desktop.MainWindow = mainWindow;

            var mainWindowViewModel = ActivatorUtilities.CreateInstance<MainWindowViewModel>(
                _serviceProvider!, 
                _serviceProvider,
                mainWindow);

            mainWindow.DataContext = mainWindowViewModel;
        }

        base.OnFrameworkInitializationCompleted();
    }
}