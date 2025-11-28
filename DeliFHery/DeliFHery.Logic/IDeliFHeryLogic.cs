using DeliFHery.Domain;
using DeliFHery.Logic.Pricing;

namespace DeliFHery.Logic;

public interface IDeliFHeryLogic
{
    Task<IEnumerable<Customer>> GetCustomersAsync();
    Task<Customer?> GetCustomerAsync(Guid customerId);
    Task AddCustomerAsync(Customer customer);
    Task UpdateCustomerAsync(Customer customer);
    Task<bool> DeleteCustomerAsync(Guid customerId);
    Task<bool> CustomerExistsAsync(Guid customerId);

    Task<IEnumerable<DeliveryOrder>> GetDeliveriesAsync();
    Task<IEnumerable<DeliveryOrder>> GetDeliveriesForCustomerAsync(Guid customerId);
    Task<DeliveryOrder?> GetDeliveryAsync(Guid deliveryId);
    Task<PriceBreakdown> CalculateDeliveryPriceAsync(DeliveryOrder deliveryOrder);
    Task AddDeliveryForCustomerAsync(Guid customerId, DeliveryOrder deliveryOrder);
    Task UpdateDeliveryAsync(DeliveryOrder deliveryOrder);
    Task<bool> DeleteDeliveryAsync(Guid deliveryId);

    Task<IEnumerable<Contact>> GetContactsForCustomerAsync(Guid customerId);
    Task<Contact?> GetContactAsync(Guid contactId);
    Task AddContactForCustomerAsync(Guid customerId, Contact contact);
    Task<bool> DeleteContactAsync(Guid contactId);
}
