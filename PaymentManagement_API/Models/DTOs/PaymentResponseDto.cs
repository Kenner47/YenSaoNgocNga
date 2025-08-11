namespace PaymentManagement_API.Models.DTOs
{
    public class PaymentResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string PaymentUrl { get; set; } = string.Empty;
        public string TransactionRef { get; set; } = string.Empty;
    }
}