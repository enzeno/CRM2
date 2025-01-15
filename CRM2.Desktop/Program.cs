using Avalonia;
using System;
using Microsoft.Extensions.DependencyInjection;
using CRM2.Desktop.Services;
using Avalonia.Controls.ApplicationLifetimes;
using Microsoft.Extensions.Hosting;
using CRM2.Desktop.Views;
using CRM2.Desktop.Features.Contacts;
using CRM2.Desktop.Features.Quotations;
using System.Net.Http;
using CRM2.Desktop.ViewModels;

namespace CRM2.Desktop;

internal class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args) => BuildAvaloniaApp()
        .StartWithClassicDesktopLifetime(args);

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace()
            .AfterSetup(builder =>
            {
                try
                {
                    Console.WriteLine("Configuring services...");
                    var services = new ServiceCollection();

                    // Configure HttpClient
                    services.AddHttpClient("API", client =>
                    {
                        client.BaseAddress = new Uri("http://localhost:5001/");
                        client.DefaultRequestHeaders.Add("Accept", "application/json");
                        Console.WriteLine($"Configured HTTP client with base address: {client.BaseAddress}");
                    });

                    // Register services
                    services.AddSingleton<IContactService, ContactService>();
                    services.AddSingleton<IQuotationService, QuotationService>();
                    services.AddSingleton<IPdfExportService, PdfExportService>();

                    // Register views
                    services.AddTransient<MainWindow>();
                    services.AddTransient<QuotationsView>();
                    services.AddTransient<ContactsView>();

                    // Register view models
                    services.AddTransient<MainWindowViewModel>();
                    services.AddTransient<QuotationsViewModel>();
                    services.AddTransient<ContactsViewModel>();

                    // Register IServiceProvider
                    services.AddSingleton<IServiceProvider>(sp => sp);

                    Console.WriteLine("Building service provider...");
                    var serviceProvider = services.BuildServiceProvider();
                    App.ServiceProvider = serviceProvider;
                    Console.WriteLine("Service provider initialized successfully");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error configuring services: {ex.GetType().Name} - {ex.Message}");
                    Console.WriteLine($"Stack trace: {ex.StackTrace}");
                    throw; // Re-throw to ensure the error is visible
                }
            });
}

public class DefaultHttpClientFactory : IHttpClientFactory
{
    private readonly Func<HttpClient> _httpClientFactory;

    public DefaultHttpClientFactory(Func<HttpClient> httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public HttpClient CreateClient(string name)
    {
        return _httpClientFactory();
    }
}
