using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Avalonia.Controls;
using System.Linq;
using System.Collections.Generic;

namespace CRM2.Desktop.Features.Contacts;

public partial class ContactsViewModel : ObservableObject
{
    private readonly IContactService _contactService;
    private readonly Window _parentWindow;
    private List<ContactDto> _allContacts = new();
    
    [ObservableProperty]
    private string _searchText = string.Empty;
    
    [ObservableProperty]
    private string _statusMessage = string.Empty;
    
    [ObservableProperty]
    private ContactDto? _selectedContact;
    
    public ObservableCollection<ContactDto> Contacts { get; } = new();

    partial void OnSearchTextChanged(string value)
    {
        FilterContacts();
    }
    
    private void FilterContacts()
    {
        var searchTerms = (SearchText ?? string.Empty)
            .ToLower()
            .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(term => term.Trim())
            .Where(term => !string.IsNullOrWhiteSpace(term))
            .ToList();

        var filtered = _allContacts;
        
        if (searchTerms.Any())
        {
            filtered = _allContacts.Where(contact =>
                searchTerms.All(term =>
                    contact.ContactId.ToLower().Contains(term) ||
                    contact.ContactType.ToLower().Contains(term) ||
                    (contact.OrganizationName?.ToLower().Contains(term) == true) ||
                    (contact.AddressLine1?.ToLower().Contains(term) == true) ||
                    (contact.AddressLine2?.ToLower().Contains(term) == true) ||
                    (contact.City?.ToLower().Contains(term) == true) ||
                    (contact.PostalCode?.ToLower().Contains(term) == true) ||
                    (contact.Country?.ToLower().Contains(term) == true) ||
                    (contact.Email?.ToLower().Contains(term) == true) ||
                    (contact.PhoneNumber?.ToLower().Contains(term) == true) ||
                    (contact.TaxId?.ToLower().Contains(term) == true)
                )
            ).ToList();
        }

        Contacts.Clear();
        foreach (var contact in filtered)
        {
            Contacts.Add(contact);
        }
        
        StatusMessage = $"Found {Contacts.Count} contacts";
    }
    
    public ContactsViewModel(IContactService contactService, Window parentWindow)
    {
        _contactService = contactService;
        _parentWindow = parentWindow;
        _ = LoadContactsAsync();
    }
    
    private async Task LoadContactsAsync()
    {
        try
        {
            var contacts = await _contactService.GetContactsAsync();
            _allContacts = contacts.ToList();
            FilterContacts();
            StatusMessage = "Contacts loaded successfully";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error loading contacts: {ex.Message}";
        }
    }
    
    [RelayCommand]
    private async Task CreateContactAsync()
    {
        var dialog = new ContactDialog(_contactService, _parentWindow);
        await dialog.ShowDialog(_parentWindow);
        await LoadContactsAsync();
    }
    
    [RelayCommand]
    private async Task EditContactAsync(ContactDto contact)
    {
        var dialog = new ContactDialog(_contactService, _parentWindow);
        dialog.DataContext = new ContactDialogViewModel(_contactService, dialog, contact);
        await dialog.ShowDialog(_parentWindow);
        await LoadContactsAsync();
    }
    
    [RelayCommand]
    private async Task DeleteContactAsync(ContactDto? contact)
    {
        if (contact == null) return;

        try
        {
            // TODO: Add confirmation dialog
            await _contactService.DeleteContactAsync(contact.ContactId);
            Contacts.Remove(contact);
            StatusMessage = "Contact deleted successfully";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error deleting contact: {ex.Message}";
        }
    }
} 