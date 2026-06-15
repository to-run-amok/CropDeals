using CropDeals.Models.Entities;

namespace CropDeals.Repositories.Interfaces
{
    public interface ICropRepository
    {
        Task<Crop?> GetByIdAsync(int id);
        Task<IEnumerable<Crop>> GetAllAsync();
        Task<IEnumerable<Crop>> GetByFarmerIdAsync(int farmerId);
        Task<Crop> CreateAsync(Crop crop);
        Task<Crop> UpdateAsync(Crop crop);
        Task DeleteAsync(Crop crop);
    }
}