using System.ComponentModel.DataAnnotations;

namespace OrderManagement_API.Models.DTOs
{
    public class UpdateTransactionDto
    {
        [StringLength(20)]
        public string? Status { get; set; }

        [StringLength(100)]
        public string? VnpTransactionNo { get; set; }

        [StringLength(10)]
        public string? VnpResponseCode { get; set; }
    }
}