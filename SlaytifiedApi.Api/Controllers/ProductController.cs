using Microsoft.AspNetCore.Mvc;
using SlaytifiedApi.Application.Interfaces;
using SlaytifiedApi.Application.Dtos;
using SlaytifiedApi.Application.Common;
using SlaytifiedApi.Domain.Entities;

namespace SlaytifiedApi.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await _productService.GetAllAsync();

            var response = new ApiResponse<object>(
                200,
                true,
                "",
                new
                {
                    page = 1,
                    totalPages = 1,
                    totalRecords = products.Count(),
                    size = 10,
                    products
                }
            );

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null)
                return NotFound(new ApiResponse<string>(404, false, "Product not found", null));

            return Ok(new ApiResponse<Product>(200, true, "", product));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProductDto productDto)
        {
            var product = new Product
            {
                Id = Guid.NewGuid(),
                Name = productDto.Name,
                Description = productDto.Description,
                Price = productDto.Price,
                ImageUrl = productDto.ImageUrl
            };

            var created = await _productService.CreateAsync(product);

            var response = new ApiResponse<Product>(
                201,
                true,
                "Product created successfully",
                created
            );

            return CreatedAtAction(nameof(GetById), new { id = created.Id }, response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] ProductDto productDto)
        {
            var product = new Product
            {
                Name = productDto.Name,
                Description = productDto.Description,
                Price = productDto.Price,
                ImageUrl = productDto.ImageUrl
            };

            var updated = await _productService.UpdateAsync(id, product);
            if (updated == null)
                return NotFound(new ApiResponse<string>(404, false, "Product not found", null));

            return Ok(new ApiResponse<Product>(200, true, "Product updated successfully", updated));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var success = await _productService.DeleteAsync(id);
            if (!success)
                return NotFound(new ApiResponse<string>(404, false, "Product not found", null));

            return Ok(new ApiResponse<string>(200, true, "Product deleted successfully", null));
        }
    }
}
