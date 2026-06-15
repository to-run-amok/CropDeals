
using CropDeals.Models.DTOs;

namespace CropDeals.Services.Interfaces
{
    public interface IFarmerService
    {
        Task<FarmerResponseDto> RegisterAsync(FarmerRegisterDto dto);
        Task<string> LoginAsync(LoginDto dto);          
        Task<FarmerResponseDto> GetProfileAsync(int id);
        Task<FarmerResponseDto> UpdateProfileAsync(int id, FarmerRegisterDto dto);
    }
}