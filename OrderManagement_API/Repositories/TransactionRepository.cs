using Microsoft.EntityFrameworkCore;
using OrderManagement_API.Data;
using OrderManagement_API.Models.Entities;
using OrderManagement_API.Repositories.IRepository;

namespace OrderManagement_API.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly AppDbContext _context;

        public TransactionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Transaction?> GetByIdAsync(int id)
        {
            return await _context.Transactions
                .Include(t => t.Order)
                .FirstOrDefaultAsync(t => t.TransactionId == id);
        }

        public async Task<Transaction?> GetByVnpTxnRefAsync(string vnpTxnRef)
        {
            return await _context.Transactions
                .Include(t => t.Order)
                .FirstOrDefaultAsync(t => t.VnpTxnRef == vnpTxnRef);
        }

        public async Task<IEnumerable<Transaction>> GetAllAsync()
        {
            return await _context.Transactions
                .Include(t => t.Order)
                .OrderByDescending(t => t.TransactionDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Transaction>> GetByOrderIdAsync(int orderId)
        {
            return await _context.Transactions
                .Where(t => t.OrderId == orderId)
                .OrderByDescending(t => t.TransactionDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Transaction>> GetByStatusAsync(string status)
        {
            return await _context.Transactions
                .Include(t => t.Order)
                .Where(t => t.Status == status)
                .OrderByDescending(t => t.TransactionDate)
                .ToListAsync();
        }

        public async Task<Transaction> CreateAsync(Transaction transaction)
        {
            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();
            return transaction;
        }

        public async Task<Transaction> UpdateAsync(Transaction transaction)
        {
            _context.Entry(transaction).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return transaction;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction == null) return false;

            _context.Transactions.Remove(transaction);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Transactions.AnyAsync(t => t.TransactionId == id);
        }

        public async Task<bool> ExistsByVnpTxnRefAsync(string vnpTxnRef)
        {
            return await _context.Transactions.AnyAsync(t => t.VnpTxnRef == vnpTxnRef);
        }
    }
}