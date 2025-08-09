using OrderManagement_API.Models.Entities;

namespace OrderManagement_API.Repositories.IRepository
{
    public interface IOrderRepository
    {
        Task<Order?> GetByIdAsync(int id);
        Task<Order?> GetByIdWithDetailsAsync(int id);
        Task<IEnumerable<Order>> GetAllAsync();
        Task<IEnumerable<Order>> GetByUserIdAsync(int userId);
        Task<IEnumerable<Order>> GetByStatusAsync(bool status);
        Task<Order> CreateAsync(Order order);
        Task<Order> UpdateAsync(Order order);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}