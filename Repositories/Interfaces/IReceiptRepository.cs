using CropDeals.Models.Entities;

namespace CropDeals.Repositories.Interfaces
{
    public interface IReceiptRepository
    {
        Task<Receipt> CreateAsync(Receipt receipt);
        Task<IEnumerable<Receipt>> GetByFarmerIdAsync(int farmerId);
        Task<Receipt?> GetByIdAsync(int id);
    }
}