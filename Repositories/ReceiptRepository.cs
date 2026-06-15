using CropDeals.Data;
using CropDeals.Models.Entities;
using CropDeals.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CropDeals.Repositories
{
    public class ReceiptRepository : IReceiptRepository
    {
        private readonly AppDbContext _context;

        public ReceiptRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Receipt> CreateAsync(Receipt receipt)
        {
            _context.Receipts.Add(receipt);
            await _context.SaveChangesAsync();
            return receipt;
        }

        public async Task<IEnumerable<Receipt>> GetByFarmerIdAsync(int farmerId) =>
            await _context.Receipts
                .Include(r => r.Crop)
                .Where(r => r.FarmerId == farmerId)
                .ToListAsync();

        public async Task<Receipt?> GetByIdAsync(int id) =>
            await _context.Receipts
                .Include(r => r.Crop)
                .FirstOrDefaultAsync(r => r.Id == id);
    }
}