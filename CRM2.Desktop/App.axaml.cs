using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using CRM2.Desktop.Views;
using Microsoft.Extensions.DependencyInjection;

namespace CRM2.Desktop;

public partial class App : Application
{
    public static IServiceProvider? ServiceProvider { get; set; }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        try
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                if (ServiceProvider == null)
                {
                    throw new InvalidOperationException("ServiceProvider is not initialized");
                }

                Console.WriteLine("Creating MainWindow...");
                var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
                desktop.MainWindow = mainWindow;
                Console.WriteLine("MainWindow created successfully");
                
                desktop.MainWindow.Show();
            }

            base.OnFrameworkInitializationCompleted();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in OnFrameworkInitializationCompleted: {ex.GetType().Name} - {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            throw;
        }
    }
}