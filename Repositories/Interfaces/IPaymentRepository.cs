using CropDeals.Models.Entities;

namespace CropDeals.Repositories.Interfaces
{
    public interface IPaymentRepository
    {
        Task<Payment> CreateAsync(Payment payment);
        Task<IEnumerable<Payment>> GetByDealerIdAsync(int dealerId);
        Task<IEnumerable<Payment>> GetByFarmerIdAsync(int farmerId);
        Task<Payment?> GetByIdAsync(int id);
    }
}