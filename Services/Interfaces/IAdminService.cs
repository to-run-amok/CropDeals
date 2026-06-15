using CropDeals.Models.DTOs;

namespace CropDeals.Services.Interfaces
{
    public interface IAdminService
    {
        Task<string> LoginAsync(AdminLoginDto dto);
        Task<IEnumerable<FarmerResponseDto>> GetAllFarmersAsync();
        Task<IEnumerable<DealerResponseDto>> GetAllDealersAsync();
        Task ToggleFarmerStatusAsync(int farmerId);
        Task ToggleDealerStatusAsync(int dealerId);

        Task<FarmerResponseDto> EditFarmerAsync(int farmerId, FarmerRegisterDto dto);
        Task<DealerResponseDto> EditDealerAsync(int dealerId, DealerRegisterDto dto);
        Task<DealerResponseDto> AddDealerAsync(DealerRegisterDto dto);
    }
}