using CropDeals.Data;
using CropDeals.Models.Entities;
using CropDeals.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CropDeals.Repositories
{
    public class SubscriptionRepository : ISubscriptionRepository
    {
        private readonly AppDbContext _context;

        public SubscriptionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Subscription> CreateAsync(Subscription subscription)
        {
            _context.Subscriptions.Add(subscription);
            await _context.SaveChangesAsync();
            return subscription;
        }

        public async Task<IEnumerable<Subscription>> GetByDealerIdAsync(int dealerId) =>
            await _context.Subscriptions
                .Where(s => s.DealerId == dealerId)
                .ToListAsync();

        public async Task<IEnumerable<Subscription>> GetByCropTypeAsync(string cropType) =>
            await _context.Subscriptions
                .Include(s => s.Dealer)
                .Where(s => s.CropType.ToLower() == cropType.ToLower())
                .ToListAsync();

        public async Task<Subscription?> GetAsync(int dealerId, string cropType) =>
            await _context.Subscriptions
                .FirstOrDefaultAsync(s => s.DealerId == dealerId &&
                    s.CropType.ToLower() == cropType.ToLower());

        public async Task DeleteAsync(Subscription subscription)
        {
            _context.Subscriptions.Remove(subscription);
            await _context.SaveChangesAsync();
        }
    }
}