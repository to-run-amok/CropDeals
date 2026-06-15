using CropDeals.Models.Entities;

namespace CropDeals.Repositories.Interfaces
{
    public interface IInvoiceRepository
    {
        Task<Invoice> CreateAsync(Invoice invoice);
        Task<IEnumerable<Invoice>> GetByDealerIdAsync(int dealerId);
        Task<Invoice?> GetByIdAsync(int id);
    }
}