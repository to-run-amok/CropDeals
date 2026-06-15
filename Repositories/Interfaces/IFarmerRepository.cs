using CropDeals.Models.Entities;

namespace CropDeals.Repositories.Interfaces
{
    public interface IFarmerRepository
    {
        Task<Farmer?> GetByEmailAsync(string email);
        Task<Farmer?> GetByIdAsync(int id);
        Task<Farmer> CreateAsync(Farmer farmer);
        Task<Farmer> UpdateAsync(Farmer farmer);
        Task<IEnumerable<Farmer>> GetAllAsync();
    }
}