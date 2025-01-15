using System;
using Avalonia.Controls;
using CRM2.Desktop.Features.Quotations;
using CRM2.Desktop.Features.Contacts;
using CRM2.Desktop.Services;
using CRM2.Desktop.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace CRM2.Desktop.Views;

public partial class MainWindow : Window
{
    private readonly IQuotationService _quotationService;
    private readonly IContactService _contactService;
    private readonly IPdfExportService _pdfExportService;

    public MainWindow()
    {
        InitializeComponent();

        try
        {
            if (App.ServiceProvider == null)
            {
                throw new InvalidOperationException("ServiceProvider is not initialized");
            }

            // Get services
            _quotationService = App.ServiceProvider.GetRequiredService<IQuotationService>();
            _contactService = App.ServiceProvider.GetRequiredService<IContactService>();
            _pdfExportService = App.ServiceProvider.GetRequiredService<IPdfExportService>();

            Console.WriteLine("Services initialized successfully");
            
            // Set DataContext to MainWindowViewModel
            DataContext = new MainWindowViewModel(
                _quotationService, 
                _contactService, 
                _pdfExportService, 
                this,
                App.ServiceProvider);
            Console.WriteLine("MainWindowViewModel initialized successfully");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error initializing MainWindow: {ex.GetType().Name} - {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            throw; // Re-throw to ensure the error is visible
        }
    }
} 