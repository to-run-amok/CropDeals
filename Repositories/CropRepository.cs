using CropDeals.Data;
using CropDeals.Models.Entities;
using CropDeals.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CropDeals.Repositories
{
    public class CropRepository : ICropRepository
    {
        private readonly AppDbContext _context;

        public CropRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Crop?> GetByIdAsync(int id) =>
            await _context.Crops
                .Include(c => c.Farmer)   // loading farmer details, who owns the crop using fkid
                .FirstOrDefaultAsync(c => c.Id == id);

        public async Task<IEnumerable<Crop>> GetAllAsync() =>
            await _context.Crops
                .Include(c => c.Farmer)
                .ToListAsync();

        public async Task<IEnumerable<Crop>> GetByFarmerIdAsync(int farmerId) =>
            await _context.Crops
                .Where(c => c.FarmerId == farmerId)
                .ToListAsync();

        public async Task<Crop> CreateAsync(Crop crop)
        {
            _context.Crops.Add(crop);
            await _context.SaveChangesAsync();
            return crop;
        }

        public async Task<Crop> UpdateAsync(Crop crop)
        {
            _context.Crops.Update(crop);
            await _context.SaveChangesAsync();
            return crop;
        }

        public async Task DeleteAsync(Crop crop)
        {
            _context.Crops.Remove(crop);
            await _context.SaveChangesAsync();
        }
    }
}