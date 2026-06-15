using CropDeals.Models.Entities;


namespace CropDeals.Repositories.Interfaces
{
    public interface ISubscriptionRepository
    {
        Task<Subscription> CreateAsync(Subscription subscription);
        Task<IEnumerable<Subscription>> GetByDealerIdAsync(int dealerId);
        Task<IEnumerable<Subscription>> GetByCropTypeAsync(string cropType);
        Task<Subscription?> GetAsync(int dealerId, string cropType);
        Task DeleteAsync(Subscription subscription);
    }
}