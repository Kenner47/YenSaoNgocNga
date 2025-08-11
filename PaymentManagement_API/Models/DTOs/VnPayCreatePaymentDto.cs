using System.ComponentModel.DataAnnotations;

namespace PaymentManagement_API.Models.DTOs
{
    public class VnPayCreatePaymentDto
    {
        [Required]
        public int OrderId { get; set; }

        [Required]
        [Range(1000, double.MaxValue, ErrorMessage = "Số tiền phải ít nhất 1,000 VND")]
        public decimal Amount { get; set; }

        [Required]
        [StringLength(200)]
        public string OrderDescription { get; set; } = string.Empty;

        [StringLength(100)]
        public string CustomerName { get; set; } = string.Empty;

        [StringLength(15)]
        public string CustomerPhone { get; set; } = string.Empty;

        [StringLength(100)]
        public string CustomerEmail { get; set; } = string.Empty;

        public string? BankCode { get; set; }
    }
}