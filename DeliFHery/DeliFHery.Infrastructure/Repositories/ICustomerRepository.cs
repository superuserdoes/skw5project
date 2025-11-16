using DeliFHery.Domain.Entities;

namespace DeliFHery.Infrastructure.Repositories;

public interface ICustomerRepository
{
    Customer? GetById(int id);
}
