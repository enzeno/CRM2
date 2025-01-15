using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Data;
using CRM2.API.Models;

namespace CRM2.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContactsController : ControllerBase
{
    private readonly string _connectionString;

    public ContactsController(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") ?? 
            throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ContactDto>>> GetContacts()
    {
        var contacts = new List<ContactDto>();

        await using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        await using var command = new MySqlCommand(
            @"SELECT contact_id, contact_type, organization_name, 
                     address_line1, address_line2, address_line3, postal_code, city, country,
                     email, phone_number, website_url, tax_id, tax_rate
              FROM contacts", connection);

        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            var taxRateOrdinal = reader.GetOrdinal("tax_rate");

            contacts.Add(new ContactDto
            {
                ContactId = reader["contact_id"]?.ToString() ?? string.Empty,
                ContactType = reader["contact_type"]?.ToString() ?? string.Empty,
                OrganizationName = reader["organization_name"]?.ToString() ?? string.Empty,
                AddressLine1 = reader["address_line1"]?.ToString() ?? string.Empty,
                AddressLine2 = reader["address_line2"]?.ToString(),
                AddressLine3 = reader["address_line3"]?.ToString(),
                PostalCode = reader["postal_code"]?.ToString() ?? string.Empty,
                City = reader["city"]?.ToString() ?? string.Empty,
                Country = reader["country"]?.ToString() ?? string.Empty,
                Email = reader["email"]?.ToString() ?? string.Empty,
                PhoneNumber = reader["phone_number"]?.ToString() ?? string.Empty,
                WebsiteUrl = reader["website_url"]?.ToString(),
                TaxId = reader["tax_id"]?.ToString() ?? string.Empty,
                TaxRate = reader.GetDecimal(taxRateOrdinal)
            });
        }

        return Ok(contacts);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ContactDto>> GetContact(string id)
    {
        await using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        await using var command = new MySqlCommand(
            @"SELECT contact_id, contact_type, organization_name, 
                     address_line1, address_line2, address_line3, postal_code, city, country,
                     email, phone_number, website_url, tax_id, tax_rate
              FROM contacts 
              WHERE contact_id = @contactId", connection);

        command.Parameters.AddWithValue("@contactId", id);

        await using var reader = await command.ExecuteReaderAsync();
        if (!await reader.ReadAsync())
            return NotFound();

        var taxRateOrdinal = reader.GetOrdinal("tax_rate");

        var contact = new ContactDto
        {
            ContactId = reader["contact_id"]?.ToString() ?? string.Empty,
            ContactType = reader["contact_type"]?.ToString() ?? string.Empty,
            OrganizationName = reader["organization_name"]?.ToString() ?? string.Empty,
            AddressLine1 = reader["address_line1"]?.ToString() ?? string.Empty,
            AddressLine2 = reader["address_line2"]?.ToString(),
            AddressLine3 = reader["address_line3"]?.ToString(),
            PostalCode = reader["postal_code"]?.ToString() ?? string.Empty,
            City = reader["city"]?.ToString() ?? string.Empty,
            Country = reader["country"]?.ToString() ?? string.Empty,
            Email = reader["email"]?.ToString() ?? string.Empty,
            PhoneNumber = reader["phone_number"]?.ToString() ?? string.Empty,
            WebsiteUrl = reader["website_url"]?.ToString(),
            TaxId = reader["tax_id"]?.ToString() ?? string.Empty,
            TaxRate = reader.GetDecimal(taxRateOrdinal)
        };

        return Ok(contact);
    }

    [HttpPost]
    public async Task<ActionResult<ContactDto>> CreateContact(ContactDto contact)
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
        command.Parameters.AddWithValue("@addressLine2", contact.AddressLine2 ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@addressLine3", contact.AddressLine3 ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@postalCode", contact.PostalCode);
        command.Parameters.AddWithValue("@city", contact.City);
        command.Parameters.AddWithValue("@country", contact.Country);
        command.Parameters.AddWithValue("@email", contact.Email ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@phoneNumber", contact.PhoneNumber ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@websiteUrl", contact.WebsiteUrl ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@taxId", contact.TaxId ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@taxRate", contact.TaxRate);

        await command.ExecuteNonQueryAsync();

        return CreatedAtAction(nameof(GetContact), new { id = contact.ContactId }, contact);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateContact(string id, ContactDto contact)
    {
        if (id != contact.ContactId)
            return BadRequest();

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
        command.Parameters.AddWithValue("@addressLine2", contact.AddressLine2 ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@addressLine3", contact.AddressLine3 ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@postalCode", contact.PostalCode);
        command.Parameters.AddWithValue("@city", contact.City);
        command.Parameters.AddWithValue("@country", contact.Country);
        command.Parameters.AddWithValue("@email", contact.Email ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@phoneNumber", contact.PhoneNumber ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@websiteUrl", contact.WebsiteUrl ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@taxId", contact.TaxId ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@taxRate", contact.TaxRate);

        var rowsAffected = await command.ExecuteNonQueryAsync();
        if (rowsAffected == 0)
            return NotFound();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteContact(string id)
    {
        await using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        await using var command = new MySqlCommand(
            "DELETE FROM contacts WHERE contact_id = @contactId", connection);

        command.Parameters.AddWithValue("@contactId", id);

        var rowsAffected = await command.ExecuteNonQueryAsync();
        if (rowsAffected == 0)
            return NotFound();

        return NoContent();
    }
} 