using Microsoft.AspNetCore.Mvc;
using PaymentManagement_API.Models.DTOs;
using PaymentManagement_API.Services.IService;

namespace PaymentManagement_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IVnPayService _vnPayService;
        private readonly ITransactionService _transactionService;

        public PaymentController(IVnPayService vnPayService, ITransactionService transactionService)
        {
            _vnPayService = vnPayService;
            _transactionService = transactionService;
        }

        [HttpPost("vnpay/create")]
        public async Task<ActionResult<VnPayPaymentResponseDto>> CreateVnPayPayment(VnPayCreatePaymentDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _vnPayService.CreatePaymentAsync(request, HttpContext);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpGet("vnpay-return")]
        public async Task<IActionResult> VnPayReturn([FromQuery] VnPayReturnDto returnData)
        {
            var transaction = await _vnPayService.ProcessReturnAsync(returnData);

            if (transaction == null)
                return BadRequest("Invalid payment return data");

            // Redirect to frontend with result
            var returnUrl = transaction.Status == "Success"
                ? $"https://localhost:3000/payment/success?txnRef={transaction.VnpTxnRef}"
                : $"https://localhost:3000/payment/failed?txnRef={transaction.VnpTxnRef}";

            return Redirect(returnUrl);
        }

        [HttpPost("vnpay-ipn")]
        public async Task<IActionResult> VnPayIpn([FromForm] VnPayReturnDto ipnData)
        {
            var transaction = await _vnPayService.ProcessIpnAsync(ipnData);

            if (transaction == null)
                return Ok(new { RspCode = "97", Message = "Invalid signature" });

            return Ok(new { RspCode = "00", Message = "Confirm success" });
        }

        [HttpGet("status/{transactionRef}")]
        public async Task<ActionResult<PaymentStatusDto>> GetPaymentStatus(string transactionRef)
        {
            var transaction = await _transactionService.GetTransactionByVnpTxnRefAsync(transactionRef);

            if (transaction == null)
                return NotFound("Transaction not found");

            return Ok(new PaymentStatusDto
            {
                Success = transaction.Status == "Success",
                Status = transaction.Status,
                Message = GetStatusMessage(transaction.Status),
                Transaction = transaction
            });
        }

        private static string GetStatusMessage(string status)
        {
            return status switch
            {
                "Pending" => "Giao dịch đang chờ xử lý",
                "Success" => "Thanh toán thành công",
                "Failed" => "Thanh toán thất bại",
                _ => "Trạng thái không xác định"
            };
        }
    }
}