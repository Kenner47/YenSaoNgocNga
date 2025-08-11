using System.ComponentModel.DataAnnotations;

namespace OrderManagement_API.Models.DTOs
{
    public class CreateOrderDto
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Tổng tiền phải lớn hơn 0")]
        public decimal TotalPrice { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Giảm giá không được âm")]
        public decimal DiscountAmount { get; set; } = 0;

        [StringLength(10)]
        public string? CouponCode { get; set; }

        [Required]
        [StringLength(100)]
        public string ShippingName { get; set; } = string.Empty;

        [Required]
        [StringLength(15)]
        public string ShippingPhone { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string ShippingAddress { get; set; } = string.Empty;

        [Required]
        public List<CreateOrderDetailDto> OrderDetails { get; set; } = new();
    }
}