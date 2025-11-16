using DeliFHery.Domain;

namespace DeliFHery.Logic;

internal class DbContact
{
    public DbContact(Guid id, Guid customerId, ContactType type, string value, bool isPrimary)
    {
        Id = id;
        CustomerId = customerId;
        Type = type;
        Value = value;
        IsPrimary = isPrimary;
    }

    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public ContactType Type { get; set; }
    public string Value { get; set; }
    public bool IsPrimary { get; set; }

    public Contact ToDomain() => new Contact(Id, CustomerId, Type, Value, IsPrimary);
}

internal static class ContactMappingExtensions
{
    public static DbContact ToDbContact(this Contact contact) => new(
        contact.Id,
        contact.CustomerId,
        contact.Type,
        contact.Value,
        contact.IsPrimary);
}
