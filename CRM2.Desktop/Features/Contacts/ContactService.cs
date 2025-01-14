using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace CRM2.Desktop.Features.Contacts;

public class ContactService
{
    private readonly string _connectionString;

    public ContactService(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<List<ContactDto>> GetContacts()
    {
        var contacts = new List<ContactDto>();

        await using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        await using var command = new MySqlCommand(
            @"SELECT contact_id, contact_type, organization_name, 
                     address_line1, address_line2, address_line3, postal_code, city, country,
                     email, phone_number, website_url, tax_id, tax_rate,
                     created_at, updated_at
              FROM contacts", connection);

        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            var taxRateOrdinal = reader.GetOrdinal("tax_rate");
            var createdAtOrdinal = reader.GetOrdinal("created_at");
            var updatedAtOrdinal = reader.GetOrdinal("updated_at");

            contacts.Add(new ContactDto
            {
                ContactId = reader["contact_id"]?.ToString() ?? string.Empty,
                ContactType = reader["contact_type"]?.ToString() ?? string.Empty,
                OrganizationName = reader["organization_name"]?.ToString() ?? string.Empty,
                AddressLine1 = reader["address_line1"]?.ToString() ?? string.Empty,
                AddressLine2 = reader["address_line2"]?.ToString() ?? string.Empty,
                AddressLine3 = reader["address_line3"]?.ToString() ?? string.Empty,
                PostalCode = reader["postal_code"]?.ToString() ?? string.Empty,
                City = reader["city"]?.ToString() ?? string.Empty,
                Country = reader["country"]?.ToString() ?? string.Empty,
                Email = reader["email"]?.ToString() ?? string.Empty,
                PhoneNumber = reader["phone_number"]?.ToString() ?? string.Empty,
                WebsiteUrl = reader["website_url"]?.ToString() ?? string.Empty,
                TaxId = reader["tax_id"]?.ToString() ?? string.Empty,
                TaxRate = reader.GetDecimal(taxRateOrdinal),
                CreatedAt = reader.GetDateTime(createdAtOrdinal),
                UpdatedAt = reader.GetDateTime(updatedAtOrdinal)
            });
        }

        return contacts;
    }

    public async Task<ContactDto?> GetContact(string contactId)
    {
        await using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        await using var command = new MySqlCommand(
            @"SELECT contact_id, contact_type, organization_name, 
                     address_line1, address_line2, address_line3, postal_code, city, country,
                     email, phone_number, website_url, tax_id, tax_rate,
                     created_at, updated_at
              FROM contacts 
              WHERE contact_id = @contactId", connection);

        command.Parameters.AddWithValue("@contactId", contactId);

        await using var reader = await command.ExecuteReaderAsync();
        if (!await reader.ReadAsync())
            return null;

        var taxRateOrdinal = reader.GetOrdinal("tax_rate");
        var createdAtOrdinal = reader.GetOrdinal("created_at");
        var updatedAtOrdinal = reader.GetOrdinal("updated_at");

        return new ContactDto
        {
            ContactId = reader["contact_id"]?.ToString() ?? string.Empty,
            ContactType = reader["contact_type"]?.ToString() ?? string.Empty,
            OrganizationName = reader["organization_name"]?.ToString() ?? string.Empty,
            AddressLine1 = reader["address_line1"]?.ToString() ?? string.Empty,
            AddressLine2 = reader["address_line2"]?.ToString() ?? string.Empty,
            AddressLine3 = reader["address_line3"]?.ToString() ?? string.Empty,
            PostalCode = reader["postal_code"]?.ToString() ?? string.Empty,
            City = reader["city"]?.ToString() ?? string.Empty,
            Country = reader["country"]?.ToString() ?? string.Empty,
            Email = reader["email"]?.ToString() ?? string.Empty,
            PhoneNumber = reader["phone_number"]?.ToString() ?? string.Empty,
            WebsiteUrl = reader["website_url"]?.ToString() ?? string.Empty,
            TaxId = reader["tax_id"]?.ToString() ?? string.Empty,
            TaxRate = reader.GetDecimal(taxRateOrdinal),
            CreatedAt = reader.GetDateTime(createdAtOrdinal),
            UpdatedAt = reader.GetDateTime(updatedAtOrdinal)
        };
    }

    public async Task<bool> AddContact(ContactDto contact)
    {
        await using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        await using var command = new MySqlCommand(
            @"INSERT INTO contacts (contact_id, contact_type, organization_name, 
                                  address_line1, address_line2, address_line3, postal_code, city, country,
                                  email, phone_number, website_url, tax_id, tax_rate)
              VALUES (@contactId, @contactType, @organizationName,
                     @addressLine1, @addressLine2, @addressLine3, @postalCode, @city, @country,
                     @email, @phoneNumber, @websiteUrl, @taxId, @taxRate)", connection);

        command.Parameters.AddWithValue("@contactId", contact.ContactId);
        command.Parameters.AddWithValue("@contactType", contact.ContactType);
        command.Parameters.AddWithValue("@organizationName", contact.OrganizationName);
        command.Parameters.AddWithValue("@addressLine1", contact.AddressLine1);
        command.Parameters.AddWithValue("@addressLine2", string.IsNullOrEmpty(contact.AddressLine2) ? DBNull.Value : contact.AddressLine2);
        command.Parameters.AddWithValue("@addressLine3", string.IsNullOrEmpty(contact.AddressLine3) ? DBNull.Value : contact.AddressLine3);
        command.Parameters.AddWithValue("@postalCode", contact.PostalCode);
        command.Parameters.AddWithValue("@city", contact.City);
        command.Parameters.AddWithValue("@country", contact.Country);
        command.Parameters.AddWithValue("@email", string.IsNullOrEmpty(contact.Email) ? DBNull.Value : contact.Email);
        command.Parameters.AddWithValue("@phoneNumber", string.IsNullOrEmpty(contact.PhoneNumber) ? DBNull.Value : contact.PhoneNumber);
        command.Parameters.AddWithValue("@websiteUrl", string.IsNullOrEmpty(contact.WebsiteUrl) ? DBNull.Value : contact.WebsiteUrl);
        command.Parameters.AddWithValue("@taxId", string.IsNullOrEmpty(contact.TaxId) ? DBNull.Value : contact.TaxId);
        command.Parameters.AddWithValue("@taxRate", contact.TaxRate);

        return await command.ExecuteNonQueryAsync() > 0;
    }

    public async Task<bool> UpdateContact(ContactDto contact)
    {
        await using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        await using var command = new MySqlCommand(
            @"UPDATE contacts 
              SET contact_type = @contactType,
                  organization_name = @organizationName,
                  address_line1 = @addressLine1,
                  address_line2 = @addressLine2,
                  address_line3 = @addressLine3,
                  postal_code = @postalCode,
                  city = @city,
                  country = @country,
                  email = @email,
                  phone_number = @phoneNumber,
                  website_url = @websiteUrl,
                  tax_id = @taxId,
                  tax_rate = @taxRate
              WHERE contact_id = @contactId", connection);

        command.Parameters.AddWithValue("@contactId", contact.ContactId);
        command.Parameters.AddWithValue("@contactType", contact.ContactType);
        command.Parameters.AddWithValue("@organizationName", contact.OrganizationName);
        command.Parameters.AddWithValue("@addressLine1", contact.AddressLine1);
        command.Parameters.AddWithValue("@addressLine2", string.IsNullOrEmpty(contact.AddressLine2) ? DBNull.Value : contact.AddressLine2);
        command.Parameters.AddWithValue("@addressLine3", string.IsNullOrEmpty(contact.AddressLine3) ? DBNull.Value : contact.AddressLine3);
        command.Parameters.AddWithValue("@postalCode", contact.PostalCode);
        command.Parameters.AddWithValue("@city", contact.City);
        command.Parameters.AddWithValue("@country", contact.Country);
        command.Parameters.AddWithValue("@email", string.IsNullOrEmpty(contact.Email) ? DBNull.Value : contact.Email);
        command.Parameters.AddWithValue("@phoneNumber", string.IsNullOrEmpty(contact.PhoneNumber) ? DBNull.Value : contact.PhoneNumber);
        command.Parameters.AddWithValue("@websiteUrl", string.IsNullOrEmpty(contact.WebsiteUrl) ? DBNull.Value : contact.WebsiteUrl);
        command.Parameters.AddWithValue("@taxId", string.IsNullOrEmpty(contact.TaxId) ? DBNull.Value : contact.TaxId);
        command.Parameters.AddWithValue("@taxRate", contact.TaxRate);

        return await command.ExecuteNonQueryAsync() > 0;
    }

    public async Task<bool> DeleteContact(string contactId)
    {
        await using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        await using var command = new MySqlCommand(
            "DELETE FROM contacts WHERE contact_id = @contactId", connection);

        command.Parameters.AddWithValue("@contactId", contactId);

        return await command.ExecuteNonQueryAsync() > 0;
    }
} 