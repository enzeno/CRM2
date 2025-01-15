using System;

namespace CRM2.Desktop.Features.Contacts;

public class Contact
{
    public string ContactId { get; set; } = string.Empty;
    public string ContactType { get; set; } = string.Empty;
    public string OrganizationName { get; set; } = string.Empty;
    public string AddressLine1 { get; set; } = string.Empty;
    public string? AddressLine2 { get; set; }
    public string? AddressLine3 { get; set; }
    public string PostalCode { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string? WebsiteUrl { get; set; }
    public string TaxId { get; set; } = string.Empty;
    public decimal TaxRate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
} 