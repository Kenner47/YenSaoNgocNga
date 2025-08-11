using OrderManagement_API.Models.DTOs;
using OrderManagement_API.Models.Entities;
using OrderManagement_API.Repositories.IRepository;
using OrderManagement_API.Services.IService;

namespace OrderManagement_API.Services
{
    public class OrderDetailService : IOrderDetailService
    {
        private readonly IOrderDetailRepository _orderDetailRepository;

        public OrderDetailService(IOrderDetailRepository orderDetailRepository)
        {
            _orderDetailRepository = orderDetailRepository;
        }

        public async Task<IEnumerable<OrderDetailDto>> GetAllOrderDetailsAsync()
        {
            var orderDetails = await _orderDetailRepository.GetAllAsync();
            return orderDetails.Select(MapToDto);
        }

        public async Task<OrderDetailDto?> GetOrderDetailByIdAsync(int id)
        {
            var orderDetail = await _orderDetailRepository.GetByIdAsync(id);
            return orderDetail == null ? null : MapToDto(orderDetail);
        }

        public async Task<IEnumerable<OrderDetailDto>> GetOrderDetailsByOrderIdAsync(int orderId)
        {
            var orderDetails = await _orderDetailRepository.GetByOrderIdAsync(orderId);
            return orderDetails.Select(MapToDto);
        }

        public async Task<IEnumerable<OrderDetailDto>> GetOrderDetailsByProductIdAsync(int productId)
        {
            var orderDetails = await _orderDetailRepository.GetByProductIdAsync(productId);
            return orderDetails.Select(MapToDto);
        }

        public async Task<OrderDetailDto> CreateOrderDetailAsync(int orderId, CreateOrderDetailDto createDto)
        {
            var orderDetail = new OrderDetail
            {
                OrderId = orderId,
                ProductId = createDto.ProductId,
                ProductName = createDto.ProductName,
                Quantity = createDto.Quantity,
                UnitPrice = createDto.UnitPrice,
                Subtotal = createDto.Quantity * createDto.UnitPrice
            };

            var createdOrderDetail = await _orderDetailRepository.CreateAsync(orderDetail);
            return MapToDto(createdOrderDetail);
        }

        public async Task<OrderDetailDto?> UpdateOrderDetailAsync(int id, UpdateOrderDetailDto updateDto)
        {
            var existingOrderDetail = await _orderDetailRepository.GetByIdAsync(id);
            if (existingOrderDetail == null) return null;

            // Cập nhật các field nếu có giá trị mới
            if (updateDto.Quantity.HasValue)
                existingOrderDetail.Quantity = updateDto.Quantity.Value;

            if (updateDto.UnitPrice.HasValue)
                existingOrderDetail.UnitPrice = updateDto.UnitPrice.Value;

            // Subtotal sẽ được tự động tính lại trong repository
            var updatedOrderDetail = await _orderDetailRepository.UpdateAsync(existingOrderDetail);
            return MapToDto(updatedOrderDetail);
        }

        public async Task<bool> DeleteOrderDetailAsync(int id)
        {
            return await _orderDetailRepository.DeleteAsync(id);
        }

        private static OrderDetailDto MapToDto(OrderDetail orderDetail)
        {
            return new OrderDetailDto
            {
                OrderDetailId = orderDetail.OrderDetailId,
                OrderId = orderDetail.OrderId,
                ProductId = orderDetail.ProductId,
                ProductName = orderDetail.ProductName,
                Quantity = orderDetail.Quantity,
                UnitPrice = orderDetail.UnitPrice,
                Subtotal = orderDetail.Subtotal
            };
        }
    }
}