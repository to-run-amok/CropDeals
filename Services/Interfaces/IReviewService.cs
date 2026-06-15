using CropDeals.Models.DTOs;

namespace CropDeals.Services.Interfaces
{
    public interface IReviewService
    {
        Task<ReviewResponseDto> AddReviewAsync(int dealerId, ReviewCreateDto dto);
        Task<IEnumerable<ReviewResponseDto>> GetFarmerReviewsAsync(int farmerId);
        Task<IEnumerable<ReviewResponseDto>> GetAllReviewsAsync();
    }
}