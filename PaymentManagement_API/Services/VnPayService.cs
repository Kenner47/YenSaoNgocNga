using Microsoft.Extensions.Options;
using PaymentManagement_API.Helpers;
using PaymentManagement_API.Models.Configurations;
using PaymentManagement_API.Models.DTOs;
using PaymentManagement_API.Services.IService;
using System.Net;
using System.Text;

namespace PaymentManagement_API.Services
{
    public class VnPayService : IVnPayService
    {
        private readonly VnPayConfig _vnPayConfig;
        private readonly ITransactionService _transactionService;

        public VnPayService(IOptions<VnPayConfig> vnPayConfig, ITransactionService transactionService)
        {
            _vnPayConfig = vnPayConfig.Value;
            _transactionService = transactionService;
        }

        public async Task<VnPayPaymentResponseDto> CreatePaymentAsync(VnPayCreatePaymentDto request, HttpContext context)
        {
            try
            {
                // Generate unique transaction reference
                var vnpTxnRef = VnPayHelper.GenerateTxnRef();
                var vnpCreateDate = DateTime.Now.ToString("yyyyMMddHHmmss");
                var vnpExpireDate = DateTime.Now.AddMinutes(15).ToString("yyyyMMddHHmmss");

                // Tạo transaction record
                var createTransactionDto = new CreateTransactionDto
                {
                    OrderId = request.OrderId,
                    VnpTxnRef = vnpTxnRef,
                    Amount = request.Amount,
                    Status = "Pending"
                };

                await _transactionService.CreateTransactionAsync(createTransactionDto);

                // Build VnPay URL
                var vnpayUrl = BuildPaymentUrl(request, vnpTxnRef, vnpCreateDate, vnpExpireDate, context);

                return new VnPayPaymentResponseDto
                {
                    Success = true,
                    PaymentUrl = vnpayUrl,
                    VnpTxnRef = vnpTxnRef,
                    Message = "Tạo URL thanh toán thành công"
                };
            }
            catch (Exception ex)
            {
                return new VnPayPaymentResponseDto
                {
                    Success = false,
                    Message = $"Lỗi tạo thanh toán: {ex.Message}"
                };
            }
        }

        public async Task<TransactionDto?> ProcessReturnAsync(VnPayReturnDto returnData)
        {
            try
            {
                // Validate signature
                if (!ValidateSignature(returnData))
                {
                    return null;
                }

                // Update transaction status
                var status = returnData.VnpResponseCode == "00" ? "Success" : "Failed";
                var updatedTransaction = await _transactionService.UpdateTransactionStatusAsync(
                    returnData.VnpTxnRef,
                    status,
                    returnData.VnpTransactionNo,
                    returnData.VnpResponseCode
                );

                return updatedTransaction;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing VnPay return: {ex.Message}");
                return null;
            }
        }

        public async Task<TransactionDto?> ProcessIpnAsync(VnPayReturnDto ipnData)
        {
            // IPN processing is similar to return processing
            return await ProcessReturnAsync(ipnData);
        }

        public bool ValidateSignature(VnPayReturnDto returnData)
        {
            try
            {
                var vnpayData = new SortedList<string, string>
                {
                    {"vnp_Amount", returnData.VnpAmount},
                    {"vnp_BankCode", returnData.VnpBankCode},
                    {"vnp_BankTranNo", returnData.VnpBankTranNo},
                    {"vnp_CardType", returnData.VnpCardType},
                    {"vnp_OrderInfo", returnData.VnpOrderInfo},
                    {"vnp_PayDate", returnData.VnpPayDate},
                    {"vnp_ResponseCode", returnData.VnpResponseCode},
                    {"vnp_TmnCode", returnData.VnpTmnCode},
                    {"vnp_TransactionNo", returnData.VnpTransactionNo},
                    {"vnp_TransactionStatus", returnData.VnpTransactionStatus},
                    {"vnp_TxnRef", returnData.VnpTxnRef}
                };

                return VnPayHelper.ValidateSignature(returnData.VnpSecureHash, _vnPayConfig.HashSecret, vnpayData);
            }
            catch
            {
                return false;
            }
        }

        private string BuildPaymentUrl(VnPayCreatePaymentDto request, string vnpTxnRef, string vnpCreateDate, string vnpExpireDate, HttpContext context)
        {
            var vnpay = new SortedList<string, string>
            {
                {"vnp_Version", _vnPayConfig.Version},
                {"vnp_Command", _vnPayConfig.Command},
                {"vnp_TmnCode", _vnPayConfig.TmnCode},
                {"vnp_Amount", ((long)(request.Amount * 100)).ToString()},
                {"vnp_CreateDate", vnpCreateDate},
                {"vnp_CurrCode", _vnPayConfig.CurrCode},
                {"vnp_IpAddr", VnPayHelper.GetIpAddress(context)},
                {"vnp_Locale", _vnPayConfig.Locale},
                {"vnp_OrderInfo", request.OrderDescription},
                {"vnp_OrderType", "other"},
                {"vnp_ReturnUrl", _vnPayConfig.ReturnUrl},
                {"vnp_TxnRef", vnpTxnRef},
                {"vnp_ExpireDate", vnpExpireDate}
            };

            if (!string.IsNullOrEmpty(request.BankCode))
            {
                vnpay.Add("vnp_BankCode", request.BankCode);
            }

            // Build query string
            StringBuilder query = new StringBuilder();
            foreach (KeyValuePair<string, string> kv in vnpay)
            {
                if (!string.IsNullOrEmpty(kv.Value))
                {
                    query.Append(WebUtility.UrlEncode(kv.Key) + "=" + WebUtility.UrlEncode(kv.Value) + "&");
                }
            }

            string queryString = query.ToString();
            if (queryString.Length > 0)
            {
                queryString = queryString.Remove(queryString.Length - 1, 1);
            }

            string signData = queryString;
            string vnpSecureHash = VnPayHelper.HmacSHA512(_vnPayConfig.HashSecret, signData);
            queryString += "&vnp_SecureHash=" + vnpSecureHash;

            return _vnPayConfig.BaseUrl + "?" + queryString;
        }
    }
}