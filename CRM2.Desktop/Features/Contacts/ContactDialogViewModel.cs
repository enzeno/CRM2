using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Avalonia.Controls;

namespace CRM2.Desktop.Features.Contacts;

public partial class ContactDialogViewModel : ObservableObject
{
    private readonly IContactService _contactService;
    private readonly Window _dialog;
    private readonly ContactDto? _existingContact;

    public IReadOnlyList<string> ContactTypes { get; } = new List<string>
    {
        "CUSTOMER",
        "SUPPLIER",
        "BOTH"
    };

    [ObservableProperty]
    private string _title;

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    [ObservableProperty]
    private string _contactId = string.Empty;

    [ObservableProperty]
    private string _contactType = string.Empty;

    [ObservableProperty]
    private string _organizationName = string.Empty;

    [ObservableProperty]
    private string _addressLine1 = string.Empty;

    [ObservableProperty]
    private string? _addressLine2;

    [ObservableProperty]
    private string? _addressLine3;

    [ObservableProperty]
    private string _postalCode = string.Empty;

    [ObservableProperty]
    private string _city = string.Empty;

    [ObservableProperty]
    private string _country = string.Empty;

    [ObservableProperty]
    private string _email = string.Empty;

    [ObservableProperty]
    private string _phoneNumber = string.Empty;

    [ObservableProperty]
    private string? _websiteUrl;

    [ObservableProperty]
    private string _taxId = string.Empty;

    [ObservableProperty]
    private decimal _taxRate;

    public ContactDialogViewModel(IContactService contactService, Window dialog, ContactDto? existingContact = null)
    {
        _contactService = contactService;
        _dialog = dialog;
        _existingContact = existingContact;
        _title = existingContact == null ? "Create New Contact" : "Edit Contact";

        if (existingContact != null)
        {
            LoadContact(existingContact);
        }
    }

    private void LoadContact(ContactDto contact)
    {
        ContactId = contact.ContactId;
        ContactType = contact.ContactType;
        OrganizationName = contact.OrganizationName;
        AddressLine1 = contact.AddressLine1;
        AddressLine2 = contact.AddressLine2;
        AddressLine3 = contact.AddressLine3;
        PostalCode = contact.PostalCode;
        City = contact.City;
        Country = contact.Country;
        Email = contact.Email;
        PhoneNumber = contact.PhoneNumber;
        WebsiteUrl = contact.WebsiteUrl;
        TaxId = contact.TaxId;
        TaxRate = contact.TaxRate;
    }

    [RelayCommand]
    private async Task Save()
    {
        if (string.IsNullOrWhiteSpace(OrganizationName))
        {
            StatusMessage = "Organization name is required";
            return;
        }

        try
        {
            var contact = new ContactDto
            {
                ContactId = ContactId,
                ContactType = ContactType,
                OrganizationName = OrganizationName,
                AddressLine1 = AddressLine1,
                AddressLine2 = AddressLine2,
                AddressLine3 = AddressLine3,
                PostalCode = PostalCode,
                City = City,
                Country = Country,
                Email = Email,
                PhoneNumber = PhoneNumber,
                WebsiteUrl = WebsiteUrl,
                TaxId = TaxId,
                TaxRate = TaxRate
            };

            bool success;
            if (_existingContact == null)
            {
                success = await _contactService.CreateContactAsync(contact);
            }
            else
            {
                success = await _contactService.UpdateContactAsync(contact);
            }

            if (success)
            {
                _dialog.Close(contact);
            }
            else
            {
                StatusMessage = "Failed to save contact";
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error saving contact: {ex.Message}";
        }
    }

    [RelayCommand]
    private void Cancel()
    {
        _dialog.Close(null);
    }
} 