using CropDeals.Data;
using CropDeals.Models.Entities;
using CropDeals.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CropDeals.Repositories
{
    public class InvoiceRepository : IInvoiceRepository
    {
        private readonly AppDbContext _context;

        public InvoiceRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Invoice> CreateAsync(Invoice invoice)
        {
            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();
            return invoice;
        }

        public async Task<IEnumerable<Invoice>> GetByDealerIdAsync(int dealerId) =>
            await _context.Invoices
                .Include(i => i.Crop)
                .Where(i => i.DealerId == dealerId)
                .ToListAsync();

        public async Task<Invoice?> GetByIdAsync(int id) =>
            await _context.Invoices
                .Include(i => i.Crop)
                .FirstOrDefaultAsync(i => i.Id == id);
    }
}