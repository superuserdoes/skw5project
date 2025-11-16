using DeliFHery.Domain;

namespace DeliFHery.Logic;

internal class ContactValidator : IEntityValidator<Contact>
{
    public void ValidateAndThrow(Contact entity)
    {
        if (entity is null)
        {
            throw new ArgumentNullException(nameof(entity));
        }

        if (entity.CustomerId == Guid.Empty)
        {
            throw new ArgumentException("Contact must be attached to a customer", nameof(entity));
        }

        if (string.IsNullOrWhiteSpace(entity.Value))
        {
            throw new ArgumentException("Contact value must be provided", nameof(entity));
        }
    }
}
