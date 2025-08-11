using Microsoft.AspNetCore.Mvc;
using PaymentManagement_API.Models.DTOs;
using PaymentManagement_API.Services.IService;

namespace PaymentManagement_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        public TransactionController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TransactionDto>>> GetAllTransactions()
        {
            var transactions = await _transactionService.GetAllTransactionsAsync();
            return Ok(transactions);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TransactionDto>> GetTransaction(int id)
        {
            var transaction = await _transactionService.GetTransactionByIdAsync(id);

            if (transaction == null)
                return NotFound($"Transaction với ID {id} không tồn tại");

            return Ok(transaction);
        }

        [HttpGet("vnpay/{vnpTxnRef}")]
        public async Task<ActionResult<TransactionDto>> GetTransactionByVnpTxnRef(string vnpTxnRef)
        {
            var transaction = await _transactionService.GetTransactionByVnpTxnRefAsync(vnpTxnRef);

            if (transaction == null)
                return NotFound($"Transaction với VnpTxnRef {vnpTxnRef} không tồn tại");

            return Ok(transaction);
        }

        [HttpGet("order/{orderId}")]
        public async Task<ActionResult<IEnumerable<TransactionDto>>> GetTransactionsByOrder(int orderId)
        {
            var transactions = await _transactionService.GetTransactionsByOrderIdAsync(orderId);
            return Ok(transactions);
        }

        [HttpGet("status/{status}")]
        public async Task<ActionResult<IEnumerable<TransactionDto>>> GetTransactionsByStatus(string status)
        {
            var transactions = await _transactionService.GetTransactionsByStatusAsync(status);
            return Ok(transactions);
        }

        [HttpPost]
        public async Task<ActionResult<TransactionDto>> CreateTransaction(CreateTransactionDto createDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var transaction = await _transactionService.CreateTransactionAsync(createDto);
                return CreatedAtAction(nameof(GetTransaction), new { id = transaction.TransactionId }, transaction);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteTransaction(int id)
        {
            var success = await _transactionService.DeleteTransactionAsync(id);

            if (!success)
                return NotFound($"Transaction với ID {id} không tồn tại");

            return NoContent();
        }
    }
}