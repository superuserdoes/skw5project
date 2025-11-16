using DeliFHery.Domain;

namespace DeliFHery.Logic;

public class DeliFHeryLogic : IDeliFHeryLogic
{
    private static readonly SemaphoreSlim Semaphore = new(1, 1);
    private static readonly Dictionary<Guid, DbCustomer> Customers = [];
    private static readonly Dictionary<Guid, DbDeliveryOrder> Deliveries = [];
    private static readonly Dictionary<Guid, DbContact> Contacts = [];
    private static bool _isSeeded;

    private readonly IEntityValidator<Customer> _customerValidator;
    private readonly IEntityValidator<DeliveryOrder> _deliveryValidator;
    private readonly IEntityValidator<Contact> _contactValidator;

    public DeliFHeryLogic(
        IEntityValidator<Customer> customerValidator,
        IEntityValidator<DeliveryOrder> deliveryValidator,
        IEntityValidator<Contact> contactValidator)
    {
        _customerValidator = customerValidator;
        _deliveryValidator = deliveryValidator;
        _contactValidator = contactValidator;

        if (!_isSeeded)
        {
            SeedData();
            _isSeeded = true;
        }
    }

    private static async Task<T> RunInLockAsync<T>(Func<T> func)
    {
        await Semaphore.WaitAsync();
        try
        {
            return func();
        }
        finally
        {
            Semaphore.Release();
        }
    }

    private static async Task DoInLockAsync(Action action)
    {
        await Semaphore.WaitAsync();
        try
        {
            action();
        }
        finally
        {
            Semaphore.Release();
        }
    }

    private static DbCustomer EnsureCustomer(Guid customerId)
    {
        if (!Customers.TryGetValue(customerId, out var customer))
        {
            throw new ArgumentException($"Customer with id {customerId} does not exist");
        }

        return customer;
    }

    private static DbDeliveryOrder EnsureDelivery(Guid deliveryId)
    {
        if (!Deliveries.TryGetValue(deliveryId, out var delivery))
        {
            throw new ArgumentException($"Delivery with id {deliveryId} does not exist");
        }

        return delivery;
    }

    private static DbContact EnsureContact(Guid contactId)
    {
        if (!Contacts.TryGetValue(contactId, out var contact))
        {
            throw new ArgumentException($"Contact with id {contactId} does not exist");
        }

        return contact;
    }

    public async Task<IEnumerable<Customer>> GetCustomersAsync()
    {
        return await RunInLockAsync(() => Customers.Values.Select(c => c.ToDomain()).ToList());
    }

    public async Task<Customer?> GetCustomerAsync(Guid customerId)
    {
        return await RunInLockAsync(() =>
        {
            Customers.TryGetValue(customerId, out var customer);
            return customer?.ToDomain();
        });
    }

    public async Task AddCustomerAsync(Customer customer)
    {
        _customerValidator.EnsureValid(customer);

        await DoInLockAsync(() =>
        {
            if (customer.Id == Guid.Empty)
            {
                customer.Id = Guid.NewGuid();
            }

            if (!Customers.TryAdd(customer.Id, customer.ToDbCustomer()))
            {
                throw new ArgumentException($"Customer with id {customer.Id} already exists");
            }
        });
    }

    public async Task UpdateCustomerAsync(Customer customer)
    {
        _customerValidator.EnsureValid(customer);

        await DoInLockAsync(() =>
        {
            var dbCustomer = EnsureCustomer(customer.Id);
            dbCustomer.Name = customer.Name;
            dbCustomer.Street = customer.Street;
            dbCustomer.City = customer.City;
            dbCustomer.PostalCode = customer.PostalCode;
            dbCustomer.Notes = customer.Notes;
        });
    }

    public async Task<bool> DeleteCustomerAsync(Guid customerId)
    {
        return await RunInLockAsync(() =>
        {
            var removed = Customers.Remove(customerId);
            if (removed)
            {
                foreach (var delivery in Deliveries.Values.Where(d => d.CustomerId == customerId).Select(d => d.Id).ToList())
                {
                    Deliveries.Remove(delivery);
                }

                foreach (var contact in Contacts.Values.Where(c => c.CustomerId == customerId).Select(c => c.Id).ToList())
                {
                    Contacts.Remove(contact);
                }
            }

            return removed;
        });
    }

    public async Task<bool> CustomerExistsAsync(Guid customerId)
    {
        return await RunInLockAsync(() => Customers.ContainsKey(customerId));
    }

    public async Task<IEnumerable<DeliveryOrder>> GetDeliveriesAsync()
    {
        return await RunInLockAsync(() => Deliveries.Values.Select(d => d.ToDomain()).ToList());
    }

    public async Task<IEnumerable<DeliveryOrder>> GetDeliveriesForCustomerAsync(Guid customerId)
    {
        return await RunInLockAsync(() => Deliveries.Values
            .Where(d => d.CustomerId == customerId)
            .Select(d => d.ToDomain())
            .ToList());
    }

    public async Task<DeliveryOrder?> GetDeliveryAsync(Guid deliveryId)
    {
        return await RunInLockAsync(() =>
        {
            Deliveries.TryGetValue(deliveryId, out var delivery);
            return delivery?.ToDomain();
        });
    }

    public async Task AddDeliveryForCustomerAsync(Guid customerId, DeliveryOrder deliveryOrder)
    {
        await DoInLockAsync(() =>
        {
            var customer = EnsureCustomer(customerId);
            deliveryOrder.CustomerId = customer.Id;
            _deliveryValidator.EnsureValid(deliveryOrder);

            if (deliveryOrder.Id == Guid.Empty)
            {
                deliveryOrder.Id = Guid.NewGuid();
            }

            if (!Deliveries.TryAdd(deliveryOrder.Id, deliveryOrder.ToDbDelivery()))
            {
                throw new ArgumentException($"Delivery with id {deliveryOrder.Id} already exists");
            }
        });
    }

    public async Task UpdateDeliveryAsync(DeliveryOrder deliveryOrder)
    {
        _deliveryValidator.EnsureValid(deliveryOrder);

        await DoInLockAsync(() =>
        {
            var dbDelivery = EnsureDelivery(deliveryOrder.Id);
            dbDelivery.OrderNumber = deliveryOrder.OrderNumber;
            dbDelivery.ScheduledAt = deliveryOrder.ScheduledAt;
            dbDelivery.DeliveredAt = deliveryOrder.DeliveredAt;
            dbDelivery.Status = deliveryOrder.Status;
        });
    }

    public async Task<bool> DeleteDeliveryAsync(Guid deliveryId)
    {
        return await RunInLockAsync(() => Deliveries.Remove(deliveryId));
    }

    public async Task<IEnumerable<Contact>> GetContactsForCustomerAsync(Guid customerId)
    {
        return await RunInLockAsync(() => Contacts.Values
            .Where(c => c.CustomerId == customerId)
            .Select(c => c.ToDomain())
            .ToList());
    }

    public async Task<Contact?> GetContactAsync(Guid contactId)
    {
        return await RunInLockAsync(() =>
        {
            Contacts.TryGetValue(contactId, out var contact);
            return contact?.ToDomain();
        });
    }

    public async Task AddContactForCustomerAsync(Guid customerId, Contact contact)
    {
        await DoInLockAsync(() =>
        {
            var customer = EnsureCustomer(customerId);
            contact.CustomerId = customer.Id;
            _contactValidator.EnsureValid(contact);

            if (contact.Id == Guid.Empty)
            {
                contact.Id = Guid.NewGuid();
            }

            if (!Contacts.TryAdd(contact.Id, contact.ToDbContact()))
            {
                throw new ArgumentException($"Contact with id {contact.Id} already exists");
            }
        });
    }

    public async Task<bool> DeleteContactAsync(Guid contactId)
    {
        return await RunInLockAsync(() => Contacts.Remove(contactId));
    }

    private static void SeedData()
    {
        if (Customers.Any())
        {
            return;
        }

        var vienna = new Customer(new Guid("11111111-2222-3333-4444-555555555555"), "Central Vienna", "Stephansplatz 1", "Wien", "1010")
        {
            Notes = "City flagship store"
        };
        Customers.Add(vienna.Id, vienna.ToDbCustomer());

        var linz = new Customer(new Guid("aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee"), "Linz Fulfillment", "Hauptplatz 5", "Linz", "4020");
        Customers.Add(linz.Id, linz.ToDbCustomer());

        var delivery1 = new DeliveryOrder(new Guid("99999999-0000-0000-0000-000000000001"), "DL-1000", DateTimeOffset.UtcNow.AddDays(-5), DeliveryStatus.Delivered, vienna.Id)
        {
            DeliveredAt = DateTimeOffset.UtcNow.AddDays(-3)
        };
        Deliveries.Add(delivery1.Id, delivery1.ToDbDelivery());

        var delivery2 = new DeliveryOrder(new Guid("99999999-0000-0000-0000-000000000002"), "DL-1001", DateTimeOffset.UtcNow.AddDays(-2), DeliveryStatus.Dispatched, linz.Id);
        Deliveries.Add(delivery2.Id, delivery2.ToDbDelivery());

        var contact1 = new Contact(new Guid("55555555-6666-7777-8888-999999999999"), vienna.Id, ContactType.Email, "vienna@delifhery.test", true);
        Contacts.Add(contact1.Id, contact1.ToDbContact());

        var contact2 = new Contact(new Guid("22222222-3333-4444-5555-666666666666"), linz.Id, ContactType.Phone, "+43 123 456789", true);
        Contacts.Add(contact2.Id, contact2.ToDbContact());
    }
}
