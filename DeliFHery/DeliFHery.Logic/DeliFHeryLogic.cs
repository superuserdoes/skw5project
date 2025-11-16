using DeliFHery.Domain;
using DeliFHery.Persistence;
using DeliFHery.Persistence.Entities;
using DeliFHery.Persistence.Mappings;
using Microsoft.EntityFrameworkCore;

namespace DeliFHery.Logic;

public class DeliFHeryLogic : IDeliFHeryLogic
{
    private readonly DeliFHeryDbContext _dbContext;
    private readonly IEntityValidator<Customer> _customerValidator;
    private readonly IEntityValidator<DeliveryOrder> _deliveryValidator;
    private readonly IEntityValidator<Contact> _contactValidator;

    public DeliFHeryLogic(
        DeliFHeryDbContext dbContext,
        IEntityValidator<Customer> customerValidator,
        IEntityValidator<DeliveryOrder> deliveryValidator,
        IEntityValidator<Contact> contactValidator)
    {
        _dbContext = dbContext;
        _customerValidator = customerValidator;
        _deliveryValidator = deliveryValidator;
        _contactValidator = contactValidator;
    }

    public async Task<IEnumerable<Customer>> GetCustomersAsync()
    {
        var customers = await _dbContext.Customers
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .ToListAsync();

        return customers.Select(c => c.ToDomain());
    }

    public async Task<Customer?> GetCustomerAsync(Guid customerId)
    {
        var entity = await _dbContext.Customers
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == customerId);

        return entity?.ToDomain();
    }

    public async Task AddCustomerAsync(Customer customer)
    {
        _customerValidator.EnsureValid(customer);

        if (customer.Id == Guid.Empty)
        {
            customer.Id = Guid.NewGuid();
        }

        await EnsureCustomerDoesNotExist(customer.Id);

        _dbContext.Customers.Add(customer.ToEntity());
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateCustomerAsync(Customer customer)
    {
        _customerValidator.EnsureValid(customer);

        var entity = await EnsureCustomerAsync(customer.Id);
        entity.Name = customer.Name;
        entity.Street = customer.Street;
        entity.City = customer.City;
        entity.PostalCode = customer.PostalCode;
        entity.Notes = customer.Notes;

        await _dbContext.SaveChangesAsync();
    }

    public async Task<bool> DeleteCustomerAsync(Guid customerId)
    {
        var entity = await _dbContext.Customers.FirstOrDefaultAsync(c => c.Id == customerId);
        if (entity is null)
        {
            return false;
        }

        _dbContext.Customers.Remove(entity);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> CustomerExistsAsync(Guid customerId)
    {
        return await _dbContext.Customers.AnyAsync(c => c.Id == customerId);
    }

    public async Task<IEnumerable<DeliveryOrder>> GetDeliveriesAsync()
    {
        var deliveries = await _dbContext.DeliveryOrders
            .AsNoTracking()
            .OrderBy(d => d.ScheduledAt)
            .ToListAsync();

        return deliveries.Select(d => d.ToDomain());
    }

    public async Task<IEnumerable<DeliveryOrder>> GetDeliveriesForCustomerAsync(Guid customerId)
    {
        var deliveries = await _dbContext.DeliveryOrders
            .AsNoTracking()
            .Where(d => d.CustomerId == customerId)
            .OrderBy(d => d.ScheduledAt)
            .ToListAsync();

        return deliveries.Select(d => d.ToDomain());
    }

    public async Task<DeliveryOrder?> GetDeliveryAsync(Guid deliveryId)
    {
        var entity = await _dbContext.DeliveryOrders
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.Id == deliveryId);

        return entity?.ToDomain();
    }

    public async Task AddDeliveryForCustomerAsync(Guid customerId, DeliveryOrder deliveryOrder)
    {
        await EnsureCustomerAsync(customerId);
        deliveryOrder.CustomerId = customerId;
        _deliveryValidator.EnsureValid(deliveryOrder);

        if (deliveryOrder.Id == Guid.Empty)
        {
            deliveryOrder.Id = Guid.NewGuid();
        }

        await EnsureDeliveryDoesNotExist(deliveryOrder.Id);

        _dbContext.DeliveryOrders.Add(deliveryOrder.ToEntity());
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateDeliveryAsync(DeliveryOrder deliveryOrder)
    {
        _deliveryValidator.EnsureValid(deliveryOrder);
        var entity = await EnsureDeliveryAsync(deliveryOrder.Id);
        entity.OrderNumber = deliveryOrder.OrderNumber;
        entity.ScheduledAt = deliveryOrder.ScheduledAt;
        entity.DeliveredAt = deliveryOrder.DeliveredAt;
        entity.Status = deliveryOrder.Status;
        entity.CustomerId = deliveryOrder.CustomerId;

        await _dbContext.SaveChangesAsync();
    }

    public async Task<bool> DeleteDeliveryAsync(Guid deliveryId)
    {
        var entity = await _dbContext.DeliveryOrders.FirstOrDefaultAsync(d => d.Id == deliveryId);
        if (entity is null)
        {
            return false;
        }

        _dbContext.DeliveryOrders.Remove(entity);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<Contact>> GetContactsForCustomerAsync(Guid customerId)
    {
        var contacts = await _dbContext.Contacts
            .AsNoTracking()
            .Where(c => c.CustomerId == customerId)
            .OrderByDescending(c => c.IsPrimary)
            .ThenBy(c => c.Type)
            .ToListAsync();

        return contacts.Select(c => c.ToDomain());
    }

    public async Task<Contact?> GetContactAsync(Guid contactId)
    {
        var entity = await _dbContext.Contacts
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == contactId);

        return entity?.ToDomain();
    }

    public async Task AddContactForCustomerAsync(Guid customerId, Contact contact)
    {
        await EnsureCustomerAsync(customerId);
        contact.CustomerId = customerId;
        _contactValidator.EnsureValid(contact);

        if (contact.Id == Guid.Empty)
        {
            contact.Id = Guid.NewGuid();
        }

        await EnsureContactDoesNotExist(contact.Id);

        _dbContext.Contacts.Add(contact.ToEntity());
        await _dbContext.SaveChangesAsync();
    }

    public async Task<bool> DeleteContactAsync(Guid contactId)
    {
        var entity = await _dbContext.Contacts.FirstOrDefaultAsync(c => c.Id == contactId);
        if (entity is null)
        {
            return false;
        }

        _dbContext.Contacts.Remove(entity);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    private async Task<CustomerEntity> EnsureCustomerAsync(Guid customerId)
    {
        var entity = await _dbContext.Customers.FirstOrDefaultAsync(c => c.Id == customerId);
        if (entity is null)
        {
            throw new ArgumentException($"Customer with id {customerId} does not exist");
        }

        return entity;
    }

    private async Task EnsureCustomerDoesNotExist(Guid customerId)
    {
        if (await _dbContext.Customers.AnyAsync(c => c.Id == customerId))
        {
            throw new ArgumentException($"Customer with id {customerId} already exists");
        }
    }

    private async Task<DeliveryOrderEntity> EnsureDeliveryAsync(Guid deliveryId)
    {
        var entity = await _dbContext.DeliveryOrders.FirstOrDefaultAsync(d => d.Id == deliveryId);
        if (entity is null)
        {
            throw new ArgumentException($"Delivery with id {deliveryId} does not exist");
        }

        return entity;
    }

    private async Task EnsureDeliveryDoesNotExist(Guid deliveryId)
    {
        if (await _dbContext.DeliveryOrders.AnyAsync(d => d.Id == deliveryId))
        {
            throw new ArgumentException($"Delivery with id {deliveryId} already exists");
        }
    }

    private async Task EnsureContactDoesNotExist(Guid contactId)
    {
        if (await _dbContext.Contacts.AnyAsync(c => c.Id == contactId))
        {
            throw new ArgumentException($"Contact with id {contactId} already exists");
        }
    }
}
