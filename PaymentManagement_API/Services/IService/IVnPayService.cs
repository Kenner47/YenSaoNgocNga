using PaymentManagement_API.Models.DTOs;

namespace PaymentManagement_API.Services.IService
{
    public interface IVnPayService
    {
        Task<VnPayPaymentResponseDto> CreatePaymentAsync(VnPayCreatePaymentDto request, HttpContext context);
        Task<TransactionDto?> ProcessReturnAsync(VnPayReturnDto returnData);
        Task<TransactionDto?> ProcessIpnAsync(VnPayReturnDto ipnData);
        bool ValidateSignature(VnPayReturnDto returnData);
    }
}