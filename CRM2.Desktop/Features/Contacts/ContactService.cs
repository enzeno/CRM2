using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace CRM2.Desktop.Features.Contacts;

public class ContactService : IContactService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private const string BaseUrl = "api/contacts";
    private const int MaxRetries = 3;
    private const int RetryDelayMs = 1000;

    public ContactService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
        Console.WriteLine("ContactService initialized");
    }

    private HttpClient CreateClient()
    {
        var client = _httpClientFactory.CreateClient("API");
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

    public async Task<List<ContactDto>> GetContactsAsync()
    {
        return await RetryOperation(async () =>
        {
            using var client = CreateClient();
            Console.WriteLine($"Fetching contacts from API at {client.BaseAddress}{BaseUrl}...");
            
            try
            {
                var contacts = await client.GetFromJsonAsync<List<ContactDto>>(BaseUrl);
                Console.WriteLine($"Received {contacts?.Count ?? 0} contacts from API");
                return contacts ?? new List<ContactDto>();
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"HTTP request failed: {ex.Message}");
                Console.WriteLine($"Status code: {ex.StatusCode}");
                throw;
            }
        }, "GetContacts");
    }

    public async Task<ContactDto?> GetContactAsync(string contactId)
    {
        return await RetryOperation(async () =>
        {
            using var client = CreateClient();
            return await client.GetFromJsonAsync<ContactDto>($"{BaseUrl}/{contactId}");
        }, $"GetContact {contactId}");
    }

    public async Task<bool> CreateContactAsync(ContactDto contact)
    {
        return await RetryOperation(async () =>
        {
            using var client = CreateClient();
            var response = await client.PostAsJsonAsync(BaseUrl, contact);
            response.EnsureSuccessStatusCode();
            return true;
        }, "CreateContact");
    }

    public async Task<bool> UpdateContactAsync(ContactDto contact)
    {
        return await RetryOperation(async () =>
        {
            using var client = CreateClient();
            var response = await client.PutAsJsonAsync($"{BaseUrl}/{contact.ContactId}", contact);
            response.EnsureSuccessStatusCode();
            return true;
        }, $"UpdateContact {contact.ContactId}");
    }

    public async Task<bool> DeleteContactAsync(string contactId)
    {
        return await RetryOperation(async () =>
        {
            using var client = CreateClient();
            var response = await client.DeleteAsync($"{BaseUrl}/{contactId}");
            response.EnsureSuccessStatusCode();
            return true;
        }, $"DeleteContact {contactId}");
    }
} 