using CropDeals.Models.Entities;

namespace CropDeals.Repositories.Interfaces
{
    public interface IReviewRepository
    {
        Task<Review> CreateAsync(Review review);
        Task<IEnumerable<Review>> GetByFarmerIdAsync(int farmerId);
        Task<IEnumerable<Review>> GetAllAsync();
        Task<bool> ExistsAsync(int dealerId, int cropId);
    }
}