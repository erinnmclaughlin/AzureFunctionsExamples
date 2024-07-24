using Microsoft.EntityFrameworkCore;

namespace FunctionsApp.Database;

public class MyDbContext(DbContextOptions<MyDbContext> options) : DbContext(options)
{
    public DbSet<Message> Messages => Set<Message>();
}
