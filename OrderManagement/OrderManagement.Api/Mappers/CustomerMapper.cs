using OrderManagement.Api.Dtos;
using OrderManagement.Domain;

namespace OrderManagement.Api.Mappers;

public static class CustomerMapper
{
    public static CustomerDto ToCustomerDto(this Customer customer)
    {
        return new CustomerDto
        {
            Id = customer.Id,
            City = customer.City,
            Name = customer.Name,
            Rating = customer.Rating,
            TotalRevenue = customer.TotalRevenue,
            ZipCode = customer.ZipCode
        };
    }
}