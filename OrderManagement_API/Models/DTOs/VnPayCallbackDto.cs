using System.ComponentModel.DataAnnotations;

namespace OrderManagement_API.Models.DTOs
{
    public class VnPayCallbackDto
    {
        [Required]
        public string VnpTxnRef { get; set; } = string.Empty;

        [Required]
        public string VnpResponseCode { get; set; } = string.Empty;

        public string? VnpTransactionNo { get; set; }

        public DateTime? VnpPayDate { get; set; }

        public string? VnpBankCode { get; set; }

        public string? VnpOrderInfo { get; set; }
    }
}