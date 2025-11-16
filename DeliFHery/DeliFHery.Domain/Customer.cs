namespace DeliFHery.Domain;

public class Customer
{
    public Customer(Guid id, string name, string street, string city, string postalCode)
    {
        Id = id;
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Street = street ?? throw new ArgumentNullException(nameof(street));
        City = city ?? throw new ArgumentNullException(nameof(city));
        PostalCode = postalCode ?? throw new ArgumentNullException(nameof(postalCode));
    }

    public Guid Id { get; set; }

    public string Name { get; set; }

    public string Street { get; set; }

    public string City { get; set; }

    public string PostalCode { get; set; }

    public string? Notes { get; set; }
}
