using CropDeals.Models.DTOs;
using CropDeals.Models.Entities;
using CropDeals.Repositories.Interfaces;
using CropDeals.Services.Interfaces;

namespace CropDeals.Services
{
    public class BankAccountService : IBankAccountService
    {
        private readonly IBankAccountRepository _repo;

        public BankAccountService(IBankAccountRepository repo)
        {
            _repo = repo;
        }

        public async Task<BankAccountResponseDto> AddOrUpdateFarmerBankAccountAsync(int farmerId, BankAccountDto dto)
        {
            var existing = await _repo.GetByFarmerIdAsync(farmerId);

            if (existing != null)
            {
                // Update if exists
                existing.AccountNumber = dto.AccountNumber;
                existing.BankName = dto.BankName;
                existing.IFSCCode = dto.IFSCCode;
                existing.AccountHolderName = dto.AccountHolderName;
                var updated = await _repo.UpdateAsync(existing);
                return MapToDto(updated);
            }

            // Create new if not
            var account = new BankAccount
            {
                AccountNumber = dto.AccountNumber,
                BankName = dto.BankName,
                IFSCCode = dto.IFSCCode,
                AccountHolderName = dto.AccountHolderName,
                FarmerId = farmerId
            };

            var created = await _repo.CreateAsync(account);
            return MapToDto(created);
        }

        public async Task<BankAccountResponseDto> GetFarmerBankAccountAsync(int farmerId)
        {
            var account = await _repo.GetByFarmerIdAsync(farmerId)
                ?? throw new KeyNotFoundException("No bank account found for this farmer.");
            return MapToDto(account);
        }

        public async Task<BankAccountResponseDto> AddOrUpdateDealerBankAccountAsync(int dealerId, BankAccountDto dto)
        {
            var existing = await _repo.GetByDealerIdAsync(dealerId);

            if (existing != null)
            {
                existing.AccountNumber = dto.AccountNumber;
                existing.BankName = dto.BankName;
                existing.IFSCCode = dto.IFSCCode;
                existing.AccountHolderName = dto.AccountHolderName;
                var updated = await _repo.UpdateAsync(existing);
                return MapToDto(updated);
            }

            var account = new BankAccount
            {
                AccountNumber = dto.AccountNumber,
                BankName = dto.BankName,
                IFSCCode = dto.IFSCCode,
                AccountHolderName = dto.AccountHolderName,
                DealerId = dealerId
            };

            var created = await _repo.CreateAsync(account);
            return MapToDto(created);
        }

        public async Task<BankAccountResponseDto> GetDealerBankAccountAsync(int dealerId)
        {
            var account = await _repo.GetByDealerIdAsync(dealerId)
                ?? throw new KeyNotFoundException("No bank account found for this dealer.");
            return MapToDto(account);
        }

        private static BankAccountResponseDto MapToDto(BankAccount b) => new()
        {
            Id = b.Id,
            AccountNumber = b.AccountNumber,
            BankName = b.BankName,
            IFSCCode = b.IFSCCode,
            AccountHolderName = b.AccountHolderName
        };
    }
}