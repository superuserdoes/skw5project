using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DeliFHery.Persistence;

public class DeliFHeryDbContextFactory : IDesignTimeDbContextFactory<DeliFHeryDbContext>
{
    public DeliFHeryDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<DeliFHeryDbContext>();
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DeliFHeryDb")
            ?? "Host=localhost;Port=5432;Database=delifhery;Username=delifhery;Password=delifhery";
        optionsBuilder.UseNpgsql(connectionString);
        return new DeliFHeryDbContext(optionsBuilder.Options);
    }
}
