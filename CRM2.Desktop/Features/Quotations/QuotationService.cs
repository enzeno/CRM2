using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace CRM2.Desktop.Features.Quotations;

public class QuotationService : IQuotationService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private const string BaseUrl = "api/quotations";
    private const int MaxRetries = 3;
    private const int RetryDelayMs = 1000;

    public QuotationService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
        Console.WriteLine("QuotationService initialized");
    }

    private HttpClient CreateClient()
    {
        var client = _httpClientFactory.CreateClient("API");
        client.DefaultRequestHeaders.Add("X-API-Key", "your-api-key");
        Console.WriteLine($"Created HTTP client with base address: {client.BaseAddress}");
        return client;
    }

    private async Task<T> RetryOperation<T>(Func<Task<T>> operation, string operationName)
    {
        Exception? lastException = null;
        for (int i = 0; i < MaxRetries; i++)
        {
            try
            {
                if (i > 0)
                {
                    Console.WriteLine($"Retry attempt {i} for {operationName}...");
                    await Task.Delay(RetryDelayMs * i);
                }
                return await operation();
            }
            catch (Exception ex)
            {
                lastException = ex;
                Console.WriteLine($"Attempt {i + 1} failed for {operationName}: {ex.Message}");
            }
        }
        throw new Exception($"Operation {operationName} failed after {MaxRetries} attempts", lastException);
    }

    public async Task<List<QuoteDto>> GetQuotesAsync()
    {
        return await RetryOperation(async () =>
        {
            using var client = CreateClient();
            var requestUrl = BaseUrl;
            Console.WriteLine($"Fetching quotes from API at {client.BaseAddress}{requestUrl}...");
            
            try
            {
                var quotes = await client.GetFromJsonAsync<List<QuoteDto>>(requestUrl);
                Console.WriteLine($"Received {quotes?.Count ?? 0} quotes from API");
                
                if (quotes != null)
                {
                    foreach (var quote in quotes)
                    {
                        Console.WriteLine($"Quote received: ID={quote.QuoteId}, Customer={quote.CustomerName}");
                    }
                }
                else
                {
                    Console.WriteLine("Received null response from API");
                }
                
                return quotes ?? new List<QuoteDto>();
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"HTTP request failed: {ex.Message}");
                Console.WriteLine($"Status code: {ex.StatusCode}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.GetType().Name} - {ex.Message}");
                throw;
            }
        }, "GetQuotes");
    }

    public async Task<QuoteDto> GetQuoteByIdAsync(string id)
    {
        return await RetryOperation(async () =>
        {
            using var client = CreateClient();
            var quote = await client.GetFromJsonAsync<QuoteDto>($"{BaseUrl}/{id}");
            return quote ?? new QuoteDto();
        }, $"GetQuote {id}");
    }

    public async Task<QuoteDto> CreateQuoteAsync(QuoteDto quote)
    {
        return await RetryOperation(async () =>
        {
            using var client = CreateClient();
            Console.WriteLine($"Creating quote for customer {quote.CustomerId}...");
            var response = await client.PostAsJsonAsync(BaseUrl, quote);
            response.EnsureSuccessStatusCode();
            var createdQuote = await response.Content.ReadFromJsonAsync<QuoteDto>();
            Console.WriteLine($"Quote created successfully with ID: {createdQuote?.QuoteId}");
            return createdQuote ?? new QuoteDto();
        }, "CreateQuote");
    }

    public async Task<QuoteDto> UpdateQuoteAsync(QuoteDto quote)
    {
        return await RetryOperation(async () =>
        {
            using var client = CreateClient();
            var response = await client.PutAsJsonAsync($"{BaseUrl}/{quote.QuoteId}", quote);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<QuoteDto>() ?? new QuoteDto();
        }, $"UpdateQuote {quote.QuoteId}");
    }

    public async Task DeleteQuoteAsync(string id)
    {
        await RetryOperation(async () =>
        {
            using var client = CreateClient();
            var response = await client.DeleteAsync($"{BaseUrl}/{id}");
            response.EnsureSuccessStatusCode();
            return true;
        }, $"DeleteQuote {id}");
    }
} 