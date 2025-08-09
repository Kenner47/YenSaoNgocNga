using OrderManagement_API.Models.DTOs;
using OrderManagement_API.Models.Entities;
using OrderManagement_API.Repositories.IRepository;
using OrderManagement_API.Services.IService;

namespace OrderManagement_API.Services
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

        public async Task<TransactionDto?> UpdateTransactionAsync(int id, UpdateTransactionDto updateDto)
        {
            var existingTransaction = await _transactionRepository.GetByIdAsync(id);
            if (existingTransaction == null) return null;

            // Cập nhật các field nếu có giá trị mới
            if (!string.IsNullOrEmpty(updateDto.Status))
                existingTransaction.Status = updateDto.Status;

            if (!string.IsNullOrEmpty(updateDto.VnpTransactionNo))
                existingTransaction.VnpTransactionNo = updateDto.VnpTransactionNo;

            if (!string.IsNullOrEmpty(updateDto.VnpResponseCode))
                existingTransaction.VnpResponseCode = updateDto.VnpResponseCode;

            var updatedTransaction = await _transactionRepository.UpdateAsync(existingTransaction);
            return MapToDto(updatedTransaction);
        }

        public async Task<TransactionDto?> UpdateTransactionByVnpCallbackAsync(VnPayCallbackDto callbackDto)
        {
            var existingTransaction = await _transactionRepository.GetByVnpTxnRefAsync(callbackDto.VnpTxnRef);
            if (existingTransaction == null) return null;

            // Cập nhật thông tin từ VnPay callback
            existingTransaction.VnpResponseCode = callbackDto.VnpResponseCode;
            existingTransaction.VnpTransactionNo = callbackDto.VnpTransactionNo;

            // Xác định trạng thái dựa trên response code
            existingTransaction.Status = callbackDto.VnpResponseCode == "00" ? "Success" : "Failed";

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
                TransactionDate = transaction.TransactionDate
            };
        }
    }
}