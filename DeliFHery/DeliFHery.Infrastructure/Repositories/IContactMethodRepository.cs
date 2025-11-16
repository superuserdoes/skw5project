using System.Collections.Generic;
using DeliFHery.Domain.Entities;

namespace DeliFHery.Infrastructure.Repositories;

public interface IContactMethodRepository
{
    IEnumerable<ContactMethod> GetByCustomerId(int customerId);
    ContactMethod Add(ContactMethod contactMethod);
    void Update(ContactMethod contactMethod);
}
