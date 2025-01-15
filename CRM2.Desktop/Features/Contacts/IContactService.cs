using System.Collections.Generic;
using System.Threading.Tasks;

namespace CRM2.Desktop.Features.Contacts;

public interface IContactService
{
    Task<List<ContactDto>> GetContactsAsync();
    Task<ContactDto?> GetContactAsync(string contactId);
    Task<bool> CreateContactAsync(ContactDto contact);
    Task<bool> UpdateContactAsync(ContactDto contact);
    Task<bool> DeleteContactAsync(string contactId);
} 