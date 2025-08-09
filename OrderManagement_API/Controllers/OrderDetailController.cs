using Microsoft.AspNetCore.Mvc;
using OrderManagement_API.Models.DTOs;
using OrderManagement_API.Services.IService;

namespace OrderManagement_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderDetailController : ControllerBase
    {
        private readonly IOrderDetailService _orderDetailService;

        public OrderDetailController(IOrderDetailService orderDetailService)
        {
            _orderDetailService = orderDetailService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDetailDto>>> GetAllOrderDetails()
        {
            var orderDetails = await _orderDetailService.GetAllOrderDetailsAsync();
            return Ok(orderDetails);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDetailDto>> GetOrderDetail(int id)
        {
            var orderDetail = await _orderDetailService.GetOrderDetailByIdAsync(id);

            if (orderDetail == null)
                return NotFound($"OrderDetail với ID {id} không tồn tại");

            return Ok(orderDetail);
        }

        [HttpGet("order/{orderId}")]
        public async Task<ActionResult<IEnumerable<OrderDetailDto>>> GetOrderDetailsByOrder(int orderId)
        {
            var orderDetails = await _orderDetailService.GetOrderDetailsByOrderIdAsync(orderId);
            return Ok(orderDetails);
        }

        [HttpGet("product/{productId}")]
        public async Task<ActionResult<IEnumerable<OrderDetailDto>>> GetOrderDetailsByProduct(int productId)
        {
            var orderDetails = await _orderDetailService.GetOrderDetailsByProductIdAsync(productId);
            return Ok(orderDetails);
        }

        [HttpPost("order/{orderId}")]
        public async Task<ActionResult<OrderDetailDto>> CreateOrderDetail(int orderId, CreateOrderDetailDto createDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var orderDetail = await _orderDetailService.CreateOrderDetailAsync(orderId, createDto);
            return CreatedAtAction(nameof(GetOrderDetail), new { id = orderDetail.OrderDetailId }, orderDetail);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<OrderDetailDto>> UpdateOrderDetail(int id, UpdateOrderDetailDto updateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var orderDetail = await _orderDetailService.UpdateOrderDetailAsync(id, updateDto);

            if (orderDetail == null)
                return NotFound($"OrderDetail với ID {id} không tồn tại");

            return Ok(orderDetail);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteOrderDetail(int id)
        {
            var success = await _orderDetailService.DeleteOrderDetailAsync(id);

            if (!success)
                return NotFound($"OrderDetail với ID {id} không tồn tại");

            return NoContent();
        }
    }
}