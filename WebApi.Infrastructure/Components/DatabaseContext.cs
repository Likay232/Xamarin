using Microsoft.EntityFrameworkCore;

namespace WebApi.Infrastructure.Components;

public class DatabaseContext(string connectionString) : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(connectionString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //TODO: Обновить после создания бд схемы
    }
}