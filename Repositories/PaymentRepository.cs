using CropDeals.Data;
using CropDeals.Models.Entities;
using CropDeals.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CropDeals.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly AppDbContext _context;

        public PaymentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Payment> CreateAsync(Payment payment)
        {
            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();
            return payment;
        }

        public async Task<IEnumerable<Payment>> GetByDealerIdAsync(int dealerId) =>
            await _context.Payments
                .Include(p => p.Farmer)
                .Include(p => p.Dealer)
                .Include(p => p.Crop)
                .Where(p => p.DealerId == dealerId)
                .ToListAsync();

        public async Task<IEnumerable<Payment>> GetByFarmerIdAsync(int farmerId) =>
            await _context.Payments
                .Include(p => p.Farmer)
                .Include(p => p.Dealer)
                .Include(p => p.Crop)
                .Where(p => p.FarmerId == farmerId)
                .ToListAsync();

        public async Task<Payment?> GetByIdAsync(int id) =>
            await _context.Payments
                .Include(p => p.Farmer)
                .Include(p => p.Dealer)
                .Include(p => p.Crop)
                .FirstOrDefaultAsync(p => p.Id == id);
    }
}