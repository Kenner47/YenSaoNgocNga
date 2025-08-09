using OrderManagement_API.Models.DTOs;

namespace OrderManagement_API.Services.IService
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderDto>> GetAllOrdersAsync();
        Task<OrderDto?> GetOrderByIdAsync(int id);
        Task<IEnumerable<OrderDto>> GetOrdersByUserIdAsync(int userId);
        Task<IEnumerable<OrderDto>> GetOrdersByStatusAsync(bool status);
        Task<OrderDto> CreateOrderAsync(CreateOrderDto createDto);
        Task<OrderDto?> UpdateOrderAsync(int id, UpdateOrderDto updateDto);
        Task<bool> DeleteOrderAsync(int id);
    }
}