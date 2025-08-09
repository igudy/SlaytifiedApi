using Microsoft.EntityFrameworkCore;
using SlaytifiedApi.Domain.Entities;

namespace SlaytifiedApi.Application.Interfaces
{
    public interface IAppDbContext
    {
        DbSet<User> Users { get; }
        DbSet<Product> Products { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
