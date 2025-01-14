using Microsoft.Extensions.Configuration;
using System.IO;

namespace CRM2.Desktop.Services;

public static class ConfigurationService
{
    private static IConfiguration? _configuration;

    public static IConfiguration Configuration
    {
        get
        {
            if (_configuration == null)
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

                _configuration = builder.Build();
            }

            return _configuration;
        }
    }
} 