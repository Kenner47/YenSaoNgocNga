using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PaymentManagement_API.Models.Entities
{
    public class Transaction
    {
        [Key]
        public int TransactionId { get; set; }

        [Required]
        public int OrderId { get; set; }

        [Required]
        [StringLength(100)]
        public string VnpTxnRef { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Pending";

        [StringLength(100)]
        public string? VnpTransactionNo { get; set; }

        [StringLength(10)]
        public string? VnpResponseCode { get; set; }

        public DateTime TransactionDate { get; set; } = DateTime.UtcNow;

    }
}
