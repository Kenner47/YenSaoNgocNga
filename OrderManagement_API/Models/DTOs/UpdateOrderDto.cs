using System.ComponentModel.DataAnnotations;

namespace OrderManagement_API.Models.DTOs
{
    public class UpdateOrderDto
    {
        [Required]
        public bool Status { get; set; }

        [StringLength(100)]
        public string? ShippingName { get; set; }

        [StringLength(15)]
        public string? ShippingPhone { get; set; }

        [StringLength(500)]
        public string? ShippingAddress { get; set; }
    }
}