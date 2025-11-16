using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xunit;
using DeliFHery.Infrastructure.Repositories;

public class CustomerRepositoryTests
{
    private const string TestConnectionString =
        "Server=localhost;Database=DeliFHeryTest;User Id=sa;Password=YourPassword123;TrustServerCertificate=True;";

    [Fact]
    public void GetById_ReturnsNull_WhenNotExists()
    {
        var repo = new CustomerRepository(TestConnectionString);

        var result = repo.GetById(-1);

        Assert.Null(result);
    }
}

