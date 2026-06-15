using CropDeals.Data;
using CropDeals.Models.Entities;
using CropDeals.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CropDeals.Repositories
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly AppDbContext _context;

        public ReviewRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Review> CreateAsync(Review review)
        {
            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();
            return review;
        }

        public async Task<IEnumerable<Review>> GetByFarmerIdAsync(int farmerId) =>
            await _context.Reviews
                .Include(r => r.Dealer)
                .Include(r => r.Farmer)
                .Include(r => r.Crop)
                .Where(r => r.FarmerId == farmerId)
                .ToListAsync();

        public async Task<IEnumerable<Review>> GetAllAsync() =>
            await _context.Reviews
                .Include(r => r.Dealer)
                .Include(r => r.Farmer)
                .Include(r => r.Crop)
                .ToListAsync();

        public async Task<bool> ExistsAsync(int dealerId, int cropId) =>
            await _context.Reviews
                .AnyAsync(r => r.DealerId == dealerId && r.CropId == cropId);
    }
}