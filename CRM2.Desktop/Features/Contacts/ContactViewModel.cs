using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace CRM2.Desktop.Features.Contacts;

public partial class ContactViewModel : ObservableObject
{
    [ObservableProperty]
    private string contactId = string.Empty;

    [ObservableProperty]
    private string contactType = string.Empty;

    [ObservableProperty]
    private string organizationName = string.Empty;

    [ObservableProperty]
    private string addressLine1 = string.Empty;

    [ObservableProperty]
    private string addressLine2 = string.Empty;

    [ObservableProperty]
    private string addressLine3 = string.Empty;

    [ObservableProperty]
    private string postalCode = string.Empty;

    [ObservableProperty]
    private string city = string.Empty;

    [ObservableProperty]
    private string country = string.Empty;

    [ObservableProperty]
    private string email = string.Empty;

    [ObservableProperty]
    private string phoneNumber = string.Empty;

    [ObservableProperty]
    private string websiteUrl = string.Empty;

    [ObservableProperty]
    private string taxId = string.Empty;

    [ObservableProperty]
    private decimal taxRate;

    public ContactViewModel()
    {
    }

    public ContactViewModel(ContactDto contact)
    {
        contactId = contact.ContactId;
        contactType = contact.ContactType;
        organizationName = contact.OrganizationName;
        addressLine1 = contact.AddressLine1;
        addressLine2 = contact.AddressLine2;
        addressLine3 = contact.AddressLine3;
        postalCode = contact.PostalCode;
        city = contact.City;
        country = contact.Country;
        email = contact.Email;
        phoneNumber = contact.PhoneNumber;
        websiteUrl = contact.WebsiteUrl;
        taxId = contact.TaxId;
        taxRate = contact.TaxRate;
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
} 