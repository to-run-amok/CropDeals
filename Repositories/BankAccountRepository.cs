using CropDeals.Data;
using CropDeals.Models.Entities;
using CropDeals.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CropDeals.Repositories
{
    public class BankAccountRepository : IBankAccountRepository
    {
        private readonly AppDbContext _context;

        public BankAccountRepository(AppDbContext context) //DI dbcontext into repo
        {
            _context = context;
        }

        public async Task<BankAccount?> GetByFarmerIdAsync(int farmerId) =>
            await _context.BankAccounts
                .FirstOrDefaultAsync(b => b.FarmerId == farmerId);

        public async Task<BankAccount?> GetByDealerIdAsync(int dealerId) =>
            await _context.BankAccounts
                .FirstOrDefaultAsync(b => b.DealerId == dealerId);      //CRUD + Ef queries + LINQ

        public async Task<BankAccount> CreateAsync(BankAccount account)
        {
            _context.BankAccounts.Add(account);
            await _context.SaveChangesAsync();
            return account;
        }

        public async Task<BankAccount> UpdateAsync(BankAccount account)
        {
            _context.BankAccounts.Update(account);
            await _context.SaveChangesAsync();
            return account;
        }
    }
}