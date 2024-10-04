using Microsoft.EntityFrameworkCore;
using PostQuery.Domain.Entities;

namespace PostQuery.Infrastructure.DBContext;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions options) : base(options)
    {
    }
    public DataContext()
    {
    }
    public DbSet<Post> Posts { get; set; }
    public DbSet<Comment> Comments { get; set; }
}