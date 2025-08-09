using ProductManagement_API.Models.Entities;

namespace ProductManagement_API.Repositories.IRepository
{
    public interface ICategoryRepository
    {
        Task<Category?> GetByIdAsync(int id);
        Task<IEnumerable<Category>> GetAllAsync();
        Task<IEnumerable<Category>> GetActiveAsync();
        Task<Category> CreateAsync(Category category);
        Task<Category> UpdateAsync(Category category);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsByNameAsync(string categoryName);
        Task<int> GetProductCountAsync(int categoryId);
    }
}