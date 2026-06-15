using CropDeals.Data;
using CropDeals.Models.Entities;
using CropDeals.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CropDeals.Repositories
{
    public class FarmerRepository : IFarmerRepository
    {
        private readonly AppDbContext _context;

  
        public FarmerRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Farmer?> GetByEmailAsync(string email)
        {
            return await _context.Farmers
                .FirstOrDefaultAsync(f => f.Email == email);
        }

        public async Task<Farmer?> GetByIdAsync(int id)
        {
            return await _context.Farmers.FindAsync(id);
        }

        public async Task<Farmer> CreateAsync(Farmer farmer)
        {
            _context.Farmers.Add(farmer);
            await _context.SaveChangesAsync();
            return farmer;
        }

        public async Task<Farmer> UpdateAsync(Farmer farmer)
        {
            _context.Farmers.Update(farmer);
            await _context.SaveChangesAsync();
            return farmer;
        }

        public async Task<IEnumerable<Farmer>> GetAllAsync()
        {
            return await _context.Farmers.ToListAsync();
        }
    }
}