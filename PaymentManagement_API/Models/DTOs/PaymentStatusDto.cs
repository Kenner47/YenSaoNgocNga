namespace PaymentManagement_API.Models.DTOs
{
    public class PaymentStatusDto
    {
        public bool Success { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public TransactionDto? Transaction { get; set; }
    }
}