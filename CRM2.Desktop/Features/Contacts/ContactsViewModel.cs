using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CRM2.Desktop.Features.Shared;
using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace CRM2.Desktop.Features.Contacts;

public partial class ContactsViewModel : ObservableObject
{
    private readonly ContactService _contactService;
    private readonly Window _owner;
    private List<ContactViewModel> _allContacts = new();

    [ObservableProperty]
    private ObservableCollection<ContactViewModel> _contacts = new();

    [ObservableProperty]
    private ContactViewModel? _selectedContact;

    private string _searchText = string.Empty;
    public string SearchText
    {
        get => _searchText;
        set
        {
            if (SetProperty(ref _searchText, value))
            {
                FilterContacts();
            }
        }
    }

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    public ContactsViewModel(Window owner)
    {
        Console.WriteLine("[ContactsViewModel] Initializing");
        _owner = owner;
        try
        {
            Console.WriteLine("[ContactsViewModel] Getting connection string from configuration");
            var connectionString = ConfigurationService.Configuration.GetConnectionString("DefaultConnection");
            if (connectionString == null)
            {
                Console.WriteLine("[ContactsViewModel] Connection string is null");
                throw new InvalidOperationException("Connection string not found");
            }
            Console.WriteLine("[ContactsViewModel] Creating ContactService");
            _contactService = new ContactService(connectionString);
            
            // Load contacts
            Console.WriteLine("[ContactsViewModel] Loading initial contacts");
            LoadContactsCommand.Execute(null);
            StatusMessage = "Connected to database successfully";
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ContactsViewModel] Error in constructor: {ex.Message}");
            Console.WriteLine($"[ContactsViewModel] Stack trace: {ex.StackTrace}");
            StatusMessage = $"Error connecting to database: {ex.Message}";
            throw;
        }
    }

    private void FilterContacts()
    {
        if (string.IsNullOrWhiteSpace(SearchText))
        {
            Contacts = new ObservableCollection<ContactViewModel>(_allContacts);
            return;
        }

        var searchTerms = SearchText.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var filteredContacts = _allContacts.Where(contact =>
        {
            var searchableText = string.Join(" ", new[]
            {
                contact.ContactType,
                contact.OrganizationName,
                contact.Email,
                contact.PhoneNumber,
                contact.City,
                contact.Country,
                contact.TaxId
            }).ToLower();

            return searchTerms.All(term => searchableText.Contains(term));
        });

        Contacts = new ObservableCollection<ContactViewModel>(filteredContacts);
    }

    [RelayCommand]
    private async Task AddContact()
    {
        Console.WriteLine("[ContactsViewModel] Adding new contact");
        try
        {
            await ContactDialog.ShowDialog(_owner, _contactService);
            await LoadContacts();
            StatusMessage = "Contact added successfully";
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ContactsViewModel] Error adding contact: {ex.Message}");
            Console.WriteLine($"[ContactsViewModel] Stack trace: {ex.StackTrace}");
            StatusMessage = $"Error adding contact: {ex.Message}";
            await MessageBox.ShowDialog(_owner, "Error", $"Failed to add contact: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task EditContact(ContactViewModel contact)
    {
        try
        {
            var contacts = await _contactService.GetContacts();
            var existingContact = contacts.FirstOrDefault(c => c.ContactId == contact.ContactId);
            if (existingContact == null) return;

            await ContactDialog.ShowDialog(_owner, _contactService, existingContact);
            await LoadContacts();
            StatusMessage = "Contact updated successfully";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error updating contact: {ex.Message}";
            await MessageBox.ShowDialog(_owner, "Error", $"Failed to update contact: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task DeleteContact(ContactViewModel contact)
    {
        try
        {
            var confirmed = await MessageBox.ShowDialog(
                _owner,
                "Confirm Delete",
                $"Are you sure you want to delete {contact.OrganizationName}?",
                true);

            if (!confirmed) return;

            await _contactService.DeleteContact(contact.ContactId);
            _allContacts.Remove(contact);
            FilterContacts();
            StatusMessage = "Contact deleted successfully";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error deleting contact: {ex.Message}";
            await MessageBox.ShowDialog(_owner, "Error", $"Failed to delete contact: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task LoadContacts()
    {
        try
        {
            var contacts = await _contactService.GetContacts();
            _allContacts = contacts.Select(contact => new ContactViewModel
            {
                ContactId = contact.ContactId,
                ContactType = contact.ContactType,
                OrganizationName = contact.OrganizationName,
                AddressLine1 = contact.AddressLine1,
                AddressLine2 = contact.AddressLine2,
                AddressLine3 = contact.AddressLine3,
                PostalCode = contact.PostalCode,
                City = contact.City,
                Country = contact.Country,
                Email = contact.Email,
                PhoneNumber = contact.PhoneNumber,
                WebsiteUrl = contact.WebsiteUrl,
                TaxId = contact.TaxId,
                TaxRate = contact.TaxRate,
                CreatedAt = contact.CreatedAt,
                UpdatedAt = contact.UpdatedAt
            }).ToList();

            FilterContacts();
            StatusMessage = $"Loaded {contacts.Count} contacts";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error loading contacts: {ex.Message}";
            await MessageBox.ShowDialog(_owner, "Error", $"Failed to load contacts: {ex.Message}");
        }
    }
} 