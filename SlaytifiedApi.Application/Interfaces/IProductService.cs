using SlaytifiedApi.Domain.Entities;

namespace SlaytifiedApi.Application.Interfaces;

public interface IProductService
{
    Task<IEnumerable<Product>> GetAllAsync();
    Task<Product?> GetByIdAsync(Guid id);
    Task<Product> CreateAsync(Product product);
    Task<Product> UpdateAsync(Guid id, Product updatedProduct);
    Task<bool> DeleteAsync(Guid id);
}