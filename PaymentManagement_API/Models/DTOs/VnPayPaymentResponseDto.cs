namespace PaymentManagement_API.Models.DTOs
{
    public class VnPayPaymentResponseDto
    {
        public bool Success { get; set; }
        public string PaymentUrl { get; set; } = string.Empty;
        public string VnpTxnRef { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}