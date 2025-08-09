using ProductManagement_API.Models.DTOs;

namespace ProductManagement_API.Services.IService
{
    public interface IProductService
    {
        Task<IEnumerable<ProductResponseDto>> GetAllProductsAsync();
        Task<ProductDto?> GetProductByIdAsync(int id);
        Task<ProductDto> CreateProductAsync(CreateProductDto createDto);
        Task<ProductDto?> UpdateProductAsync(int id, UpdateProductDto updateDto);
        Task<bool> DeleteProductAsync(int id);
        Task<IEnumerable<ProductResponseDto>> SearchProductsAsync(string searchTerm);
        Task<IEnumerable<ProductResponseDto>> GetProductsByCategoryAsync(int categoryId);
    }
}