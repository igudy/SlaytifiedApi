using Microsoft.EntityFrameworkCore;
using SlaytifiedApi.Domain.Entities;
using SlaytifiedApi.Infrastructure.Data;
using SlaytifiedApi.Application.Interfaces;

namespace SlaytifiedApi.Application.Services;

public class ProductService : IProductService
{
    private readonly AppDbContext _context;

    public ProductService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Product>> GetAllAsync() => await _context.Products.OrderByDescending(p => p.CreatedAt).ToListAsync();

    public async Task<ProductService?> GetByIdAsync(Guid id) => await _context.Products.FindAsync(id);

    public async Task<Product> CreateAsync(ProductService product)
    {
        product.Id = Guid.NewGuid();
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        return product;
    }

    public async Task<Product> UpdateAsync(Guid id, ProductService updatedProduct)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null) return null;

        product.Name = updatedProduct.Name;
        product.Description = updatedProduct.Description;
        product.Price = updatedProduct.Price;
        product.ImageUrl = updatedProduct.ImageUrl;

        await _context.SaveChangesAsync();
        return product;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null) return false;

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
        return true;
    }

}