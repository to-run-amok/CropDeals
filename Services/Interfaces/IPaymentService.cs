using CropDeals.Models.DTOs;

namespace CropDeals.Services.Interfaces
{
    public interface IPaymentService
    {
        Task<PaymentResponseDto> MakePaymentAsync(int dealerId, PaymentRequestDto dto);
        Task<IEnumerable<PaymentResponseDto>> GetDealerPaymentsAsync(int dealerId);
        Task<IEnumerable<PaymentResponseDto>> GetFarmerPaymentsAsync(int farmerId);
    }
}