using CropDeals.Models.Entities;

namespace CropDeals.Repositories.Interfaces
{
    public interface IBankAccountRepository
    {
        Task<BankAccount?> GetByFarmerIdAsync(int farmerId);
        Task<BankAccount?> GetByDealerIdAsync(int dealerId);
        Task<BankAccount> CreateAsync(BankAccount account);
        Task<BankAccount> UpdateAsync(BankAccount account);
    }
}