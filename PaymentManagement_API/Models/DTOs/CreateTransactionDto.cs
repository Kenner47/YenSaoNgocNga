using System.ComponentModel.DataAnnotations;

namespace PaymentManagement_API.Models.DTOs
{
    public class CreateTransactionDto
    {
        [Required]
        public int OrderId { get; set; }

        [Required]
        [StringLength(100)]
        public string VnpTxnRef { get; set; } = string.Empty;

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Số tiền phải lớn hơn 0")]
        public decimal Amount { get; set; }

        [StringLength(20)]
        public string Status { get; set; } = "Pending";

        [StringLength(100)]
        public string? VnpTransactionNo { get; set; }

        [StringLength(10)]
        public string? VnpResponseCode { get; set; }
    }
}