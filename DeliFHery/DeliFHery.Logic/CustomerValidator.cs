using DeliFHery.Domain;

namespace DeliFHery.Logic;

internal class CustomerValidator : IEntityValidator<Customer>
{
    public void ValidateAndThrow(Customer entity)
    {
        if (entity is null)
        {
            throw new ArgumentNullException(nameof(entity));
        }

        if (string.IsNullOrWhiteSpace(entity.Name))
        {
            throw new ArgumentException("Customer name must be provided", nameof(entity));
        }

        if (string.IsNullOrWhiteSpace(entity.Street))
        {
            throw new ArgumentException("Customer street must be provided", nameof(entity));
        }

        if (string.IsNullOrWhiteSpace(entity.City))
        {
            throw new ArgumentException("Customer city must be provided", nameof(entity));
        }

        if (string.IsNullOrWhiteSpace(entity.PostalCode))
        {
            throw new ArgumentException("Customer postal code must be provided", nameof(entity));
        }
    }
}
