namespace DeliFHery.Domain;

public class Contact
{
    public Contact(Guid id, Guid customerId, ContactType type, string value, bool isPrimary)
    {
        Id = id;
        CustomerId = customerId;
        Type = type;
        Value = value ?? throw new ArgumentNullException(nameof(value));
        IsPrimary = isPrimary;
    }

    public Guid Id { get; set; }

    public Guid CustomerId { get; set; }

    public ContactType Type { get; set; }

    public string Value { get; set; }

    public bool IsPrimary { get; set; }
}
