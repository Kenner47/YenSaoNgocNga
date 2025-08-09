namespace OrderManagement_API.Models.DTOs
{
    public class OrderDto
    {
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public DateOnly OrderDate { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal DiscountAmount { get; set; }
        public string? CouponCode { get; set; }
        public bool Status { get; set; }
        public string ShippingName { get; set; } = string.Empty;
        public string ShippingPhone { get; set; } = string.Empty;
        public string ShippingAddress { get; set; } = string.Empty;
        public List<OrderDetailDto> OrderDetails { get; set; } = new();

    }
}
