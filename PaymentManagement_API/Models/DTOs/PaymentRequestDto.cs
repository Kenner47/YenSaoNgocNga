namespace PaymentManagement_API.Models.DTOs
{
    public class PaymentRequestDto
    {
        public int OrderId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "VND";
        public string PaymentMethod { get; set; } = "VnPay";
        public string OrderDescription { get; set; } = string.Empty;
        public string CustomerInfo { get; set; } = string.Empty;
        public string ReturnUrl { get; set; } = string.Empty;
        public string CallbackUrl { get; set; } = string.Empty;
    }
}
