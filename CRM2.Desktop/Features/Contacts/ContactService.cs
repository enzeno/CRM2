using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace CRM2.Desktop.Features.Contacts;

public class ContactService : IContactService
{
    private readonly HttpClient _httpClient;

    public ContactService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<ContactDto>> GetContactsAsync()
    {
        var response = await _httpClient.GetAsync("api/contacts");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<ContactDto>>() ?? new List<ContactDto>();
    }

    public async Task<ContactDto?> GetContactAsync(string contactId)
    {
        var response = await _httpClient.GetAsync($"api/contacts/{contactId}");
        if (!response.IsSuccessStatusCode)
            return null;
        return await response.Content.ReadFromJsonAsync<ContactDto>();
    }

    public async Task<bool> CreateContactAsync(ContactDto contact)
    {
        var response = await _httpClient.PostAsJsonAsync("api/contacts", contact);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateContactAsync(ContactDto contact)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/contacts/{contact.ContactId}", contact);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteContactAsync(string contactId)
    {
        var response = await _httpClient.DeleteAsync($"api/contacts/{contactId}");
        return response.IsSuccessStatusCode;
    }
} 