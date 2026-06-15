using CropDeals.Models.DTOs;

namespace CropDeals.Services.Interfaces
{
    public interface ICropService
    {
        Task<CropResponseDto> PostCropAsync(int farmerId, CropCreateDto dto);
        Task<IEnumerable<CropResponseDto>> GetAllCropsAsync();
        Task<CropResponseDto> GetCropByIdAsync(int id);
        Task<IEnumerable<CropResponseDto>> GetMycropsAsync(int farmerId);
        Task<CropResponseDto> MarkAsSoldAsync(int cropId, int farmerId);
        Task DeleteCropAsync(int cropId, int farmerId);
    }
}