namespace PaymentManagement_API.Models.DTOs
{
    public class TransactionDto
    {
        public int TransactionId { get; set; }
        public int OrderId { get; set; }
        public string VnpTxnRef { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? VnpTransactionNo { get; set; }
        public string? VnpResponseCode { get; set; }
        public string? VnpBankCode { get; set; }
        public string? VnpOrderInfo { get; set; }
        public DateTime? VnpPayDate { get; set; }
        public DateTime TransactionDate { get; set; }
    }
}