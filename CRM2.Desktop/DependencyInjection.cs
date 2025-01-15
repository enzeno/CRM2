using System;
using Microsoft.Extensions.DependencyInjection;
using CRM2.Desktop.Features.Quotations;
using CRM2.Desktop.Features.Contacts;
using CRM2.Desktop.ViewModels;

namespace CRM2.Desktop;

public static class DependencyInjection
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        // Register view models
        services.AddTransient<MainWindowViewModel>();
        services.AddTransient<ContactsViewModel>();
        services.AddTransient<QuotationsViewModel>();

        // Configure HTTP clients
        services.AddHttpClient<IQuotationService, QuotationService>(client =>
        {
            client.BaseAddress = new Uri("http://localhost:5001");
        });

        services.AddHttpClient<IContactService, ContactService>(client =>
        {
            client.BaseAddress = new Uri("http://localhost:5001");
        });

        return services;
    }
} 