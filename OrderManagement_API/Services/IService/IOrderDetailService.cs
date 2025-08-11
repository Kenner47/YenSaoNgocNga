using OrderManagement_API.Models.DTOs;

namespace OrderManagement_API.Services.IService
{
    public interface IOrderDetailService
    {
        Task<IEnumerable<OrderDetailDto>> GetAllOrderDetailsAsync();
        Task<OrderDetailDto?> GetOrderDetailByIdAsync(int id);
        Task<IEnumerable<OrderDetailDto>> GetOrderDetailsByOrderIdAsync(int orderId);
        Task<IEnumerable<OrderDetailDto>> GetOrderDetailsByProductIdAsync(int productId);
        Task<OrderDetailDto> CreateOrderDetailAsync(int orderId, CreateOrderDetailDto createDto);
        Task<OrderDetailDto?> UpdateOrderDetailAsync(int id, UpdateOrderDetailDto updateDto);
        Task<bool> DeleteOrderDetailAsync(int id);
    }
}