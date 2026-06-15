using CropDeals.Models.DTOs;

namespace CropDeals.Services.Interfaces
{
    public interface IBankAccountService
    {
        Task<BankAccountResponseDto> AddOrUpdateFarmerBankAccountAsync(int farmerId, BankAccountDto dto);
        Task<BankAccountResponseDto> GetFarmerBankAccountAsync(int farmerId);
        Task<BankAccountResponseDto> AddOrUpdateDealerBankAccountAsync(int dealerId, BankAccountDto dto);
        Task<BankAccountResponseDto> GetDealerBankAccountAsync(int dealerId);
    }
}