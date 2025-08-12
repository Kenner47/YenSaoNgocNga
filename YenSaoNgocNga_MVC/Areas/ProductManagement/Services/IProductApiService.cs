using YenSaoNgocNga_MVC.Areas.ProductManagement.Models;

namespace YenSaoNgocNga_MVC.Areas.ProductManagement.Services
{
    public interface IProductApiService
    {
        Task<IEnumerable<ProductViewModel>> GetAllProductsAsync();
        Task<ProductViewModel?> GetProductByIdAsync(int id);
        Task<IEnumerable<ProductViewModel>> GetProductsByCategoryAsync(int categoryId);
        Task<IEnumerable<ProductViewModel>> SearchProductsAsync(string query);
        Task<IEnumerable<CategoryViewModel>> GetCategoriesAsync();
    }
}