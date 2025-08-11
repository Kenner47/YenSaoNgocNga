using ProductManagement_API.Models.DTOs;
using ProductManagement_API.Models.Entities;
using ProductManagement_API.Repositories;
using ProductManagement_API.Repositories.IRepository;
using ProductManagement_API.Services.IService;

namespace ProductManagement_API.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IInventoryService _inventoryService;

        public ProductService(IProductRepository productRepository, IInventoryService inventoryService)
        {
            _productRepository = productRepository;
            _inventoryService = inventoryService;
        }

        public async Task<ProductDto?> GetProductByIdAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null) return null;

            return MapToDetailDto(product);
        }

        public async Task<IEnumerable<ProductResponseDto>> GetAllProductsAsync()
        {
            var products = await _productRepository.GetAllAsync();
            return products.Select(MapToSummaryDto);
        }

        public async Task<IEnumerable<ProductResponseDto>> GetProductsByCategoryAsync(int categoryId)
        {
            var products = await _productRepository.GetByCategoryAsync(categoryId);
            return products.Select(MapToSummaryDto);
        }

        public async Task<IEnumerable<ProductResponseDto>> SearchProductsAsync(string searchTerm)
        {
            var products = await _productRepository.SearchAsync(searchTerm);
            return products.Select(MapToSummaryDto);
        }

        public async Task<ProductDto> CreateProductAsync(CreateProductDto createDto)
        {
            var product = new Product
            {
                ProductName = createDto.ProductName,
                Description = createDto.Description,
                Price = createDto.Price,
                CategoryId = createDto.CategoryId,
                ProductType = createDto.ProductType,
                Origin = createDto.Origin,
                Weight = createDto.Weight,
                Grade = createDto.Grade,
                ImageUrl = createDto.ImageUrl,
                IsActive = true
            };

            var createdProduct = await _productRepository.CreateAsync(product);

            if (createDto.InitialStock > 0)
            {
                await _inventoryService.CreateInventoryAsync(createdProduct.ProductId, createDto.InitialStock);

                createdProduct = await _productRepository.GetByIdAsync(createdProduct.ProductId);
            }

            return MapToDetailDto(createdProduct);
        }

        public async Task<ProductDto?> UpdateProductAsync(int id, UpdateProductDto updateDto)
        {
            var existingProduct = await _productRepository.GetByIdAsync(id);
            if (existingProduct == null) return null;

            if (updateDto.ProductName != null)
                existingProduct.ProductName = updateDto.ProductName;
            if (updateDto.Description != null)
                existingProduct.Description = updateDto.Description;
            if (updateDto.Price.HasValue)
                existingProduct.Price = updateDto.Price.Value;
            if (updateDto.CategoryId.HasValue)
                existingProduct.CategoryId = updateDto.CategoryId.Value;
            if (updateDto.ProductType != null)
                existingProduct.ProductType = updateDto.ProductType;
            if (updateDto.Origin != null)
                existingProduct.Origin = updateDto.Origin;
            if (updateDto.Weight.HasValue)
                existingProduct.Weight = updateDto.Weight.Value;
            if (updateDto.Grade != null)
                existingProduct.Grade = updateDto.Grade;
            if (updateDto.ImageUrl != null)
                existingProduct.ImageUrl = updateDto.ImageUrl;
            if (updateDto.IsActive.HasValue)
                existingProduct.IsActive = updateDto.IsActive.Value;

            var updatedProduct = await _productRepository.UpdateAsync(existingProduct);
            return MapToDetailDto(updatedProduct);
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            return await _productRepository.DeleteAsync(id);
        }

        private static ProductDto MapToDetailDto(Product product)
        {
            return new ProductDto
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                Description = product.Description,
                Price = product.Price,
                CategoryId = product.CategoryId,
                CategoryName = product.Category?.CategoryName ?? "",
                ProductType = product.ProductType,
                Origin = product.Origin,
                Weight = product.Weight,
                Grade = product.Grade,
                ImageUrl = product.ImageUrl,
                IsActive = product.IsActive,
                CreatedAt = product.CreatedAt,
                Inventory = product.Inventory != null ? MapInventoryToDto(product.Inventory) : null
            };
        }

        private static ProductResponseDto MapToSummaryDto(Product product)
        {
            return new ProductResponseDto
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                Price = product.Price,
                CategoryName = product.Category?.CategoryName ?? "",
                ProductType = product.ProductType,
                ImageUrl = product.ImageUrl,
                IsActive = product.IsActive,
                StockQuantity = product.Inventory?.Quantity ?? 0,
                StockStatus = GetStockStatus(product.Inventory)
            };
        }

        private static InventoryDto MapInventoryToDto(Inventory inventory)
        {
            return new InventoryDto
            {
                InventoryId = inventory.InventoryId,
                ProductId = inventory.ProductId,
                Quantity = inventory.Quantity,
                MinStockLevel = inventory.MinStockLevel,
                IsLowStock = inventory.Quantity <= inventory.MinStockLevel,
                IsOutOfStock = inventory.Quantity <= 0,
                StockStatus = GetStockStatus(inventory),
                LastUpdated = inventory.UpdatedAt,
                UpdatedBy = inventory.UpdatedBy,
                Notes = inventory.Notes
            };
        }

        private static string GetStockStatus(Inventory? inventory)
        {
            if (inventory == null) return "N/A";
            if (inventory.Quantity <= 0) return "Hết hàng";
            if (inventory.Quantity <= inventory.MinStockLevel) return "Sắp hết";
            return "Còn hàng";
        }
    }
}