using PaymentManagement_API.Models.DTOs;
using PaymentManagement_API.Models.Entities;
using PaymentManagement_API.Repositories.IRepository;
using PaymentManagement_API.Services.IService;

namespace PaymentManagement_API.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _transactionRepository;

        public TransactionService(ITransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;
        }

        public async Task<IEnumerable<TransactionDto>> GetAllTransactionsAsync()
        {
            var transactions = await _transactionRepository.GetAllAsync();
            return transactions.Select(MapToDto);
        }

        public async Task<TransactionDto?> GetTransactionByIdAsync(int id)
        {
            var transaction = await _transactionRepository.GetByIdAsync(id);
            return transaction == null ? null : MapToDto(transaction);
        }

        public async Task<TransactionDto?> GetTransactionByVnpTxnRefAsync(string vnpTxnRef)
        {
            var transaction = await _transactionRepository.GetByVnpTxnRefAsync(vnpTxnRef);
            return transaction == null ? null : MapToDto(transaction);
        }

        public async Task<IEnumerable<TransactionDto>> GetTransactionsByOrderIdAsync(int orderId)
        {
            var transactions = await _transactionRepository.GetByOrderIdAsync(orderId);
            return transactions.Select(MapToDto);
        }

        public async Task<IEnumerable<TransactionDto>> GetTransactionsByStatusAsync(string status)
        {
            var transactions = await _transactionRepository.GetByStatusAsync(status);
            return transactions.Select(MapToDto);
        }

        public async Task<TransactionDto> CreateTransactionAsync(CreateTransactionDto createDto)
        {
            // Kiểm tra VnpTxnRef đã tồn tại chưa
            var existingTransaction = await _transactionRepository.ExistsByVnpTxnRefAsync(createDto.VnpTxnRef);
            if (existingTransaction)
            {
                throw new InvalidOperationException($"Transaction với VnpTxnRef {createDto.VnpTxnRef} đã tồn tại");
            }

            var transaction = new Transaction
            {
                OrderId = createDto.OrderId,
                VnpTxnRef = createDto.VnpTxnRef,
                Amount = createDto.Amount,
                Status = createDto.Status,
                VnpTransactionNo = createDto.VnpTransactionNo,
                VnpResponseCode = createDto.VnpResponseCode,
                TransactionDate = DateTime.UtcNow
            };

            var createdTransaction = await _transactionRepository.CreateAsync(transaction);
            return MapToDto(createdTransaction);
        }

        public async Task<TransactionDto?> UpdateTransactionStatusAsync(string vnpTxnRef, string status, string? vnpTransactionNo = null, string? vnpResponseCode = null)
        {
            var existingTransaction = await _transactionRepository.GetByVnpTxnRefAsync(vnpTxnRef);
            if (existingTransaction == null) return null;

            // Cập nhật thông tin từ VnPay
            existingTransaction.Status = status;
            if (!string.IsNullOrEmpty(vnpTransactionNo))
                existingTransaction.VnpTransactionNo = vnpTransactionNo;
            if (!string.IsNullOrEmpty(vnpResponseCode))
                existingTransaction.VnpResponseCode = vnpResponseCode;

            var updatedTransaction = await _transactionRepository.UpdateAsync(existingTransaction);
            return MapToDto(updatedTransaction);
        }

        public async Task<bool> DeleteTransactionAsync(int id)
        {
            return await _transactionRepository.DeleteAsync(id);
        }

        private static TransactionDto MapToDto(Transaction transaction)
        {
            return new TransactionDto
            {
                TransactionId = transaction.TransactionId,
                OrderId = transaction.OrderId,
                VnpTxnRef = transaction.VnpTxnRef,
                Amount = transaction.Amount,
                Status = transaction.Status,
                VnpTransactionNo = transaction.VnpTransactionNo,
                VnpResponseCode = transaction.VnpResponseCode,
                VnpBankCode = transaction.VnpBankCode,
                VnpOrderInfo = transaction.VnpOrderInfo,
                VnpPayDate = transaction.VnpPayDate,
                TransactionDate = transaction.TransactionDate
            };
        }
    }
}