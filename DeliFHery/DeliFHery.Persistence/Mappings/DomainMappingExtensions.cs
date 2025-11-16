using DeliFHery.Domain;
using DeliFHery.Persistence.Entities;

namespace DeliFHery.Persistence.Mappings;

public static class DomainMappingExtensions
{
    public static CustomerEntity ToEntity(this Customer customer)
    {
        return new CustomerEntity
        {
            Id = customer.Id,
            Name = customer.Name,
            Street = customer.Street,
            City = customer.City,
            PostalCode = customer.PostalCode,
            Notes = customer.Notes
        };
    }

    public static Customer ToDomain(this CustomerEntity entity)
    {
        return new Customer(entity.Id, entity.Name, entity.Street, entity.City, entity.PostalCode)
        {
            Notes = entity.Notes
        };
    }

    public static DeliveryOrderEntity ToEntity(this DeliveryOrder deliveryOrder)
    {
        return new DeliveryOrderEntity
        {
            Id = deliveryOrder.Id,
            OrderNumber = deliveryOrder.OrderNumber,
            ScheduledAt = deliveryOrder.ScheduledAt,
            DeliveredAt = deliveryOrder.DeliveredAt,
            Status = deliveryOrder.Status,
            CustomerId = deliveryOrder.CustomerId
        };
    }

    public static DeliveryOrder ToDomain(this DeliveryOrderEntity entity)
    {
        return new DeliveryOrder(entity.Id, entity.OrderNumber, entity.ScheduledAt, entity.Status, entity.CustomerId)
        {
            DeliveredAt = entity.DeliveredAt
        };
    }

    public static ContactEntity ToEntity(this Contact contact)
    {
        return new ContactEntity
        {
            Id = contact.Id,
            CustomerId = contact.CustomerId,
            Type = contact.Type,
            Value = contact.Value,
            IsPrimary = contact.IsPrimary
        };
    }

    public static Contact ToDomain(this ContactEntity entity)
    {
        return new Contact(entity.Id, entity.CustomerId, entity.Type, entity.Value, entity.IsPrimary);
    }
}
