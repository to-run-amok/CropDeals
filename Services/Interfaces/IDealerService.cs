using CropDeals.Models.DTOs;

namespace CropDeals.Services.Interfaces
{
    public interface IDealerService
    {
        Task<DealerResponseDto> RegisterAsync(DealerRegisterDto dto);
        Task<string> LoginAsync(LoginDto dto);
        Task<DealerResponseDto> GetProfileAsync(int id);
        Task<DealerResponseDto> UpdateProfileAsync(int id, DealerRegisterDto dto);
        Task<IEnumerable<DealerResponseDto>> GetAllAsync();
    }
}