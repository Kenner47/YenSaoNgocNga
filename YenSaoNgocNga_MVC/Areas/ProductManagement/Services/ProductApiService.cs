using YenSaoNgocNga_MVC.Areas.ProductManagement.Models;
using YenSaoNgocNga_MVC.Models.Configuration;
using System.Text.Json;

namespace YenSaoNgocNga_MVC.Areas.ProductManagement.Services
{
    public class ProductApiService : IProductApiService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        public ProductApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(ApiSettings.ApiUrls.ProductManagementApi);
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        public async Task<IEnumerable<ProductViewModel>> GetAllProductsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("/api/product");

                if (response.IsSuccessStatusCode)
                {
                    var jsonContent = await response.Content.ReadAsStringAsync();
                    var products = JsonSerializer.Deserialize<List<ProductApiResponse>>(jsonContent, _jsonOptions);

                    return products?.Select(MapToViewModel) ?? new List<ProductViewModel>();
                }

                return new List<ProductViewModel>();
            }
            catch (Exception ex)
            {
                // Log error
                Console.WriteLine($"Error getting products: {ex.Message}");
                return new List<ProductViewModel>();
            }
        }

        public async Task<ProductViewModel?> GetProductByIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/product/{id}");

                if (response.IsSuccessStatusCode)
                {
                    var jsonContent = await response.Content.ReadAsStringAsync();
                    var product = JsonSerializer.Deserialize<ProductApiResponse>(jsonContent, _jsonOptions);

                    return product != null ? MapToViewModel(product) : null;
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting product {id}: {ex.Message}");
                return null;
            }
        }

        public async Task<IEnumerable<ProductViewModel>> GetProductsByCategoryAsync(int categoryId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/product/category/{categoryId}");

                if (response.IsSuccessStatusCode)
                {
                    var jsonContent = await response.Content.ReadAsStringAsync();
                    var products = JsonSerializer.Deserialize<List<ProductApiResponse>>(jsonContent, _jsonOptions);

                    return products?.Select(MapToViewModel) ?? new List<ProductViewModel>();
                }

                return new List<ProductViewModel>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting products by category {categoryId}: {ex.Message}");
                return new List<ProductViewModel>();
            }
        }

        public async Task<IEnumerable<ProductViewModel>> SearchProductsAsync(string query)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/product/search?q={Uri.EscapeDataString(query)}");

                if (response.IsSuccessStatusCode)
                {
                    var jsonContent = await response.Content.ReadAsStringAsync();
                    var products = JsonSerializer.Deserialize<List<ProductApiResponse>>(jsonContent, _jsonOptions);

                    return products?.Select(MapToViewModel) ?? new List<ProductViewModel>();
                }

                return new List<ProductViewModel>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error searching products: {ex.Message}");
                return new List<ProductViewModel>();
            }
        }

        public async Task<IEnumerable<CategoryViewModel>> GetCategoriesAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("/api/category");

                if (response.IsSuccessStatusCode)
                {
                    var jsonContent = await response.Content.ReadAsStringAsync();
                    var categories = JsonSerializer.Deserialize<List<CategoryApiResponse>>(jsonContent, _jsonOptions);

                    return categories?.Select(c => new CategoryViewModel
                    {
                        CategoryId = c.CategoryId,
                        CategoryName = c.CategoryName,
                        Description = c.Description ?? string.Empty
                    }) ?? new List<CategoryViewModel>();
                }

                return new List<CategoryViewModel>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting categories: {ex.Message}");
                return new List<CategoryViewModel>();
            }
        }

        private static ProductViewModel MapToViewModel(ProductApiResponse product)
        {
            return new ProductViewModel
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                Description = product.Description,
                Price = product.Price,
                ProductType = product.ProductType,
                Origin = product.Origin,
                Grade = product.Grade,
                Weight = product.Weight,
                ImageUrl = product.ImageUrl,
                IsActive = product.IsActive,
                CategoryName = product.CategoryName,
                CategoryId = product.CategoryId,
                CreatedAt = product.CreatedAt,
                StockQuantity = product.Inventory?.Quantity ?? 0,
                StockStatus = GetStockStatus(product.Inventory?.Quantity ?? 0)
            };
        }

        private static string GetStockStatus(int quantity)
        {
            return quantity switch
            {
                > 50 => "Còn hàng",
                > 10 => "Sắp hết",
                > 0 => "Còn ít",
                _ => "Hết hàng"
            };
        }
    }

    // API Response Models
    public class ProductApiResponse
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string ProductType { get; set; } = string.Empty;
        public string Origin { get; set; } = string.Empty;
        public string Grade { get; set; } = string.Empty;
        public int Weight { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public DateTime CreatedAt { get; set; }
        public InventoryInfo? Inventory { get; set; }
    }

    public class InventoryInfo
    {
        public int Quantity { get; set; }
        public string StockStatus { get; set; } = string.Empty;
    }

    public class CategoryApiResponse
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}