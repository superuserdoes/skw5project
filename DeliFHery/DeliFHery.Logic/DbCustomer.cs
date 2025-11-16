using DeliFHery.Domain;

namespace DeliFHery.Logic;

internal class DbCustomer
{
    public DbCustomer(Guid id, string name, string street, string city, string postalCode, string? notes)
    {
        Id = id;
        Name = name;
        Street = street;
        City = city;
        PostalCode = postalCode;
        Notes = notes;
    }

    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Street { get; set; }
    public string City { get; set; }
    public string PostalCode { get; set; }
    public string? Notes { get; set; }

    public Customer ToDomain()
    {
        var customer = new Customer(Id, Name, Street, City, PostalCode)
        {
            Notes = Notes
        };
        return customer;
    }
}

internal static class CustomerMappingExtensions
{
    public static DbCustomer ToDbCustomer(this Customer customer) => new(
        customer.Id,
        customer.Name,
        customer.Street,
        customer.City,
        customer.PostalCode,
        customer.Notes);
}
