using DeliFHery.Api.Dtos;
using DeliFHery.Domain;

namespace DeliFHery.Api.Mappers;

public static class CustomerMapper
{
    public static CustomerDto ToDto(this Customer customer)
    {
        ArgumentNullException.ThrowIfNull(customer);

        return new CustomerDto(
            customer.Id,
            customer.Name,
            customer.Street,
            customer.City,
            customer.PostalCode,
            customer.Notes);
    }

    public static Customer ToDomain(this CustomerRequest request, Guid customerId)
    {
        ArgumentNullException.ThrowIfNull(request);

        var customer = new Customer(
            customerId,
            request.Name!,
            request.Street!,
            request.City!,
            request.PostalCode!)
        {
            Notes = request.Notes
        };

        return customer;
    }
}
