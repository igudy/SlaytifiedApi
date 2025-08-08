using Microsoft.EntityFrameworkCore;
using SlaytifiedApi.Domain.Entities;
using SlaytifiedApi.Infrasturcture.Data;

namespace SlaytifiedApi.Infrasturcture.Data;

public class AppDbContext : AppDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    
    public DbSet<Product> Products => Set<Product>();
}