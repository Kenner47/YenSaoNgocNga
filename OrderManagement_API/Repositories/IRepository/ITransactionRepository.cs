using OrderManagement_API.Models.Entities;

namespace OrderManagement_API.Repositories.IRepository
{
    public interface ITransactionRepository
    {
        Task<Transaction?> GetByIdAsync(int id);
        Task<Transaction?> GetByVnpTxnRefAsync(string vnpTxnRef);
        Task<IEnumerable<Transaction>> GetAllAsync();
        Task<IEnumerable<Transaction>> GetByOrderIdAsync(int orderId);
        Task<IEnumerable<Transaction>> GetByStatusAsync(string status);
        Task<Transaction> CreateAsync(Transaction transaction);
        Task<Transaction> UpdateAsync(Transaction transaction);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<bool> ExistsByVnpTxnRefAsync(string vnpTxnRef);
    }
}