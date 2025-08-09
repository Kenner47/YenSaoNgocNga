using OrderManagement_API.Models.DTOs;
using OrderManagement_API.Models.Entities;
using OrderManagement_API.Repositories.IRepository;
using OrderManagement_API.Services.IService;

namespace OrderManagement_API.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;

        public OrderService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<IEnumerable<OrderDto>> GetAllOrdersAsync()
        {
            var orders = await _orderRepository.GetAllAsync();
            return orders.Select(MapToDto);
        }

        public async Task<OrderDto?> GetOrderByIdAsync(int id)
        {
            var order = await _orderRepository.GetByIdWithDetailsAsync(id);
            return order == null ? null : MapToDto(order);
        }

        public async Task<IEnumerable<OrderDto>> GetOrdersByUserIdAsync(int userId)
        {
            var orders = await _orderRepository.GetByUserIdAsync(userId);
            return orders.Select(MapToDto);
        }

        public async Task<IEnumerable<OrderDto>> GetOrdersByStatusAsync(bool status)
        {
            var orders = await _orderRepository.GetByStatusAsync(status);
            return orders.Select(MapToDto);
        }

        public async Task<OrderDto> CreateOrderAsync(CreateOrderDto createDto)
        {
            var order = new Order
            {
                UserId = createDto.UserId,
                OrderDate = DateOnly.FromDateTime(DateTime.UtcNow),
                TotalPrice = createDto.TotalPrice,
                DiscountAmount = createDto.DiscountAmount,
                CouponCode = createDto.CouponCode,
                Status = false, // Mặc định là pending
                ShippingName = createDto.ShippingName,
                ShippingPhone = createDto.ShippingPhone,
                ShippingAddress = createDto.ShippingAddress
            };

            // Tạo OrderDetails
            foreach (var detailDto in createDto.OrderDetails)
            {
                var orderDetail = new OrderDetail
                {
                    ProductId = detailDto.ProductId,
                    ProductName = detailDto.ProductName,
                    Quantity = detailDto.Quantity,
                    UnitPrice = detailDto.UnitPrice,
                    Subtotal = detailDto.Quantity * detailDto.UnitPrice
                };
                order.OrderDetails.Add(orderDetail);
            }

            var createdOrder = await _orderRepository.CreateAsync(order);

            // Load lại với details để return đầy đủ
            var orderWithDetails = await _orderRepository.GetByIdWithDetailsAsync(createdOrder.OrderId);
            return MapToDto(orderWithDetails!);
        }

        public async Task<OrderDto?> UpdateOrderAsync(int id, UpdateOrderDto updateDto)
        {
            var existingOrder = await _orderRepository.GetByIdAsync(id);
            if (existingOrder == null) return null;

            existingOrder.Status = updateDto.Status;

            if (!string.IsNullOrEmpty(updateDto.ShippingName))
                existingOrder.ShippingName = updateDto.ShippingName;
            if (!string.IsNullOrEmpty(updateDto.ShippingPhone))
                existingOrder.ShippingPhone = updateDto.ShippingPhone;
            if (!string.IsNullOrEmpty(updateDto.ShippingAddress))
                existingOrder.ShippingAddress = updateDto.ShippingAddress;

            var updatedOrder = await _orderRepository.UpdateAsync(existingOrder);

            // Load lại với details
            var orderWithDetails = await _orderRepository.GetByIdWithDetailsAsync(updatedOrder.OrderId);
            return MapToDto(orderWithDetails!);
        }

        public async Task<bool> DeleteOrderAsync(int id)
        {
            return await _orderRepository.DeleteAsync(id);
        }

        private static OrderDto MapToDto(Order order)
        {
            return new OrderDto
            {
                OrderId = order.OrderId,
                UserId = order.UserId,
                OrderDate = order.OrderDate,
                TotalPrice = order.TotalPrice,
                DiscountAmount = order.DiscountAmount,
                CouponCode = order.CouponCode,
                Status = order.Status,
                ShippingName = order.ShippingName,
                ShippingPhone = order.ShippingPhone,
                ShippingAddress = order.ShippingAddress,
                OrderDetails = order.OrderDetails.Select(MapOrderDetailToDto).ToList()
            };
        }

        private static OrderDetailDto MapOrderDetailToDto(OrderDetail orderDetail)
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