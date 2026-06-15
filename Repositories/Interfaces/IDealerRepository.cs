using CropDeals.Models.Entities;

namespace CropDeals.Repositories.Interfaces
{
    public interface IDealerRepository
    {
        Task<Dealer?> GetByEmailAsync(string email);
        Task<Dealer?> GetByIdAsync(int id);
        Task<Dealer> CreateAsync(Dealer dealer);
        Task<Dealer> UpdateAsync(Dealer dealer);
        Task<IEnumerable<Dealer>> GetAllAsync();
    }
}