using DeliFHery.Api.Dtos;
using DeliFHery.Domain;

namespace DeliFHery.Api.Mappers;

public static class ContactMapper
{
    public static ContactDto ToDto(this Contact contact)
    {
        ArgumentNullException.ThrowIfNull(contact);

        return new ContactDto(
            contact.Id,
            contact.CustomerId,
            contact.Type,
            contact.Value,
            contact.IsPrimary);
    }

    public static Contact ToDomain(this ContactRequest request, Guid contactId, Guid customerId)
    {
        ArgumentNullException.ThrowIfNull(request);

        return new Contact(
            contactId,
            customerId,
            request.Type,
            request.Value!,
            request.IsPrimary);
    }
}
