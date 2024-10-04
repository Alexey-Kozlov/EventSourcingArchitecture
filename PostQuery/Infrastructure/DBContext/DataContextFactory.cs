using Microsoft.EntityFrameworkCore;

namespace PostQuery.Infrastructure.DBContext;

public class DataContextFactory
{
    private readonly Action<DbContextOptionsBuilder> _configureDbContext;

    public DataContextFactory(Action<DbContextOptionsBuilder> configureDbContext)
    {
        _configureDbContext = configureDbContext;
    }
    public DataContext CreateDbContext()
    {
        DbContextOptionsBuilder<DataContext> options = new();
        _configureDbContext(options);
        return new DataContext(options.Options);
    }
}