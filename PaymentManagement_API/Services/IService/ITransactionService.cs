using PaymentManagement_API.Models.DTOs;

namespace PaymentManagement_API.Services.IService
{
    public interface ITransactionService
    {
        Task<IEnumerable<TransactionDto>> GetAllTransactionsAsync();
        Task<TransactionDto?> GetTransactionByIdAsync(int id);
        Task<TransactionDto?> GetTransactionByVnpTxnRefAsync(string vnpTxnRef);
        Task<IEnumerable<TransactionDto>> GetTransactionsByOrderIdAsync(int orderId);
        Task<IEnumerable<TransactionDto>> GetTransactionsByStatusAsync(string status);
        Task<TransactionDto> CreateTransactionAsync(CreateTransactionDto createDto);
        Task<TransactionDto?> UpdateTransactionStatusAsync(string vnpTxnRef, string status, string? vnpTransactionNo = null, string? vnpResponseCode = null);
        Task<bool> DeleteTransactionAsync(int id);
    }
}