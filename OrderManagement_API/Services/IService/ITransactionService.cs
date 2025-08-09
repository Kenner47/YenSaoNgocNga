using OrderManagement_API.Models.DTOs;

namespace OrderManagement_API.Services.IService
{
    public interface ITransactionService
    {
        Task<IEnumerable<TransactionDto>> GetAllTransactionsAsync();
        Task<TransactionDto?> GetTransactionByIdAsync(int id);
        Task<TransactionDto?> GetTransactionByVnpTxnRefAsync(string vnpTxnRef);
        Task<IEnumerable<TransactionDto>> GetTransactionsByOrderIdAsync(int orderId);
        Task<IEnumerable<TransactionDto>> GetTransactionsByStatusAsync(string status);
        Task<TransactionDto> CreateTransactionAsync(CreateTransactionDto createDto);
        Task<TransactionDto?> UpdateTransactionAsync(int id, UpdateTransactionDto updateDto);
        Task<TransactionDto?> UpdateTransactionByVnpCallbackAsync(VnPayCallbackDto callbackDto);
        Task<bool> DeleteTransactionAsync(int id);
    }
}