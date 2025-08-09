using OrderManagement_API.Models.Entities;

namespace OrderManagement_API.Repositories.IRepository
{
    public interface IOrderDetailRepository
    {
        Task<OrderDetail?> GetByIdAsync(int id);
        Task<IEnumerable<OrderDetail>> GetAllAsync();
        Task<IEnumerable<OrderDetail>> GetByOrderIdAsync(int orderId);
        Task<IEnumerable<OrderDetail>> GetByProductIdAsync(int productId);
        Task<OrderDetail> CreateAsync(OrderDetail orderDetail);
        Task<OrderDetail> UpdateAsync(OrderDetail orderDetail);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}