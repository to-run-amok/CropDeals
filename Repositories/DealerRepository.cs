using CropDeals.Data;
using CropDeals.Models.Entities;
using CropDeals.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CropDeals.Repositories
{
    public class DealerRepository : IDealerRepository
    {
        private readonly AppDbContext _context;

        public DealerRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Dealer?> GetByEmailAsync(string email) =>
            await _context.Dealers.FirstOrDefaultAsync(d => d.Email == email);

        public async Task<Dealer?> GetByIdAsync(int id) =>
            await _context.Dealers.FindAsync(id);

        public async Task<Dealer> CreateAsync(Dealer dealer)
        {
            _context.Dealers.Add(dealer);
            await _context.SaveChangesAsync();
            return dealer;
        }

        public async Task<Dealer> UpdateAsync(Dealer dealer)
        {
            _context.Dealers.Update(dealer);
            await _context.SaveChangesAsync();
            return dealer;
        }

        public async Task<IEnumerable<Dealer>> GetAllAsync() =>
            await _context.Dealers.ToListAsync();
    }
}