using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CRM2.Desktop.Features.Shared;

namespace CRM2.Desktop.Features.Contacts;

public partial class ContactDialogViewModel : ObservableObject
{
    private readonly Window _owner;
    private readonly ContactService _contactService;
    private readonly ContactDto? _existingContact;

    [ObservableProperty]
    private string _title;

    [ObservableProperty]
    private string _contactId = string.Empty;

    [ObservableProperty]
    private string _contactType = "CUSTOMER";

    [ObservableProperty]
    private string _organizationName = string.Empty;

    [ObservableProperty]
    private string _addressLine1 = string.Empty;

    [ObservableProperty]
    private string _addressLine2 = string.Empty;

    [ObservableProperty]
    private string _addressLine3 = string.Empty;

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
    private string _websiteUrl = string.Empty;

    [ObservableProperty]
    private string _taxId = string.Empty;

    [ObservableProperty]
    private decimal _taxRate = 0.0m;

    public List<string> ContactTypes { get; } = new() { "CUSTOMER", "SUPPLIER" };

    public ContactDialogViewModel(Window owner, ContactService contactService, ContactDto? existingContact = null)
    {
        _owner = owner;
        _contactService = contactService;
        _existingContact = existingContact;
        _title = existingContact == null ? "Add Contact" : "Edit Contact";

        if (existingContact != null)
        {
            ContactId = existingContact.ContactId;
            ContactType = existingContact.ContactType;
            OrganizationName = existingContact.OrganizationName;
            AddressLine1 = existingContact.AddressLine1;
            AddressLine2 = existingContact.AddressLine2;
            AddressLine3 = existingContact.AddressLine3;
            PostalCode = existingContact.PostalCode;
            City = existingContact.City;
            Country = existingContact.Country;
            Email = existingContact.Email;
            PhoneNumber = existingContact.PhoneNumber;
            WebsiteUrl = existingContact.WebsiteUrl;
            TaxId = existingContact.TaxId;
            TaxRate = existingContact.TaxRate;
        }
    }

    [RelayCommand]
    private async Task Save(Window window)
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

        try
        {
            if (_existingContact == null)
            {
                await _contactService.AddContact(contact);
            }
            else
            {
                await _contactService.UpdateContact(contact);
            }
            window.Close();
        }
        catch (Exception ex)
        {
            await MessageBox.ShowDialog(_owner, "Error", $"Failed to save contact: {ex.Message}");
        }
    }

    [RelayCommand]
    private void Cancel(Window window)
    {
        window.Close();
    }
} 