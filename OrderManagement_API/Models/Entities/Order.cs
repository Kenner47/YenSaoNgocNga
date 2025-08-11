using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderManagement_API.Models.Entities
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public DateOnly OrderDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalPrice { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal DiscountAmount { get; set; } = 0;

        [StringLength(10)]
        public string? CouponCode { get; set; }

        [Required]
        public bool Status { get; set; } = false;

        // Shipping Info
        [Required]
        [StringLength(100)]
        public string ShippingName { get; set; } = string.Empty;

        [Required]
        [StringLength(15)]
        public string ShippingPhone { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string ShippingAddress { get; set; } = string.Empty;

        // Navigation Properties
        public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }
}