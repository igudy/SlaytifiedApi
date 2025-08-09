using Microsoft.EntityFrameworkCore;
using SlaytifiedApi.Application.Interfaces;
using SlaytifiedApi.Domain.Entities;

namespace SlaytifiedApi.Infrastructure.Data
{
    public class AppDbContext : DbContext, IAppDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Product> Products => Set<Product>();
    }
}
