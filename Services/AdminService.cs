using CropDeals.Helpers;
using CropDeals.Models.DTOs;
using CropDeals.Models.Entities;
using CropDeals.Repositories.Interfaces;
using CropDeals.Services.Interfaces;

namespace CropDeals.Services
{
    public class AdminService : IAdminService
    {
        private readonly IFarmerRepository _farmerRepo;
        private readonly IDealerRepository _dealerRepo;
        private readonly JwtHelper _jwtHelper;
        private readonly IConfiguration _config;
        private readonly ILogger<AdminService> _logger;

        public AdminService(
            IFarmerRepository farmerRepo,
            IDealerRepository dealerRepo,
            JwtHelper jwtHelper,
            IConfiguration config,
            ILogger<AdminService> logger)
        {
            _farmerRepo = farmerRepo;
            _dealerRepo = dealerRepo;
            _jwtHelper = jwtHelper;
            _config = config;
            _logger = logger;
        }

        public Task<string> LoginAsync(AdminLoginDto dto)
        {
            _logger.LogInformation("Admin login attempt for username: {Username}", dto.Username);

            var adminUsername = _config["Admin:Username"];
            var adminPassword = _config["Admin:Password"];

            if (dto.Username != adminUsername || dto.Password != adminPassword)
            {
                _logger.LogWarning("Admin login failed for username: {Username}", dto.Username);
                throw new UnauthorizedAccessException("Invalid admin credentials.");
            }

            _logger.LogInformation("Admin logged in successfully");
            var token = _jwtHelper.GenerateToken(0, dto.Username, "Admin");
            return Task.FromResult(token);
        }

        public async Task<IEnumerable<FarmerResponseDto>> GetAllFarmersAsync()
        {
            _logger.LogInformation("Admin fetching all farmers");
            var farmers = await _farmerRepo.GetAllAsync();
            return farmers.Select(f => new FarmerResponseDto
            {
                Id = f.Id,
                Name = f.Name,
                Email = f.Email,
                Phone = f.Phone
            });
        }

        public async Task<IEnumerable<DealerResponseDto>> GetAllDealersAsync()
        {
            _logger.LogInformation("Admin fetching all dealers");
            var dealers = await _dealerRepo.GetAllAsync();
            return dealers.Select(d => new DealerResponseDto
            {
                Id = d.Id,
                Name = d.Name,
                Email = d.Email,
                Phone = d.Phone,
                IsActive = d.IsActive

            });
        }

        public async Task ToggleFarmerStatusAsync(int farmerId)
        {
            _logger.LogInformation("Admin toggling status for farmer ID: {Id}", farmerId);

            var farmer = await _farmerRepo.GetByIdAsync(farmerId)
                ?? throw new KeyNotFoundException($"Farmer with ID {farmerId} not found.");

            farmer.IsActive = !farmer.IsActive;
            await _farmerRepo.UpdateAsync(farmer);

            _logger.LogInformation("Farmer ID: {Id} is now {Status}", farmerId,
                farmer.IsActive ? "Active" : "Inactive");
        }

        public async Task ToggleDealerStatusAsync(int dealerId)
        {
            _logger.LogInformation("Admin toggling status for dealer ID: {Id}", dealerId);

            var dealer = await _dealerRepo.GetByIdAsync(dealerId)
                ?? throw new KeyNotFoundException($"Dealer with ID {dealerId} not found.");

            dealer.IsActive = !dealer.IsActive;
            await _dealerRepo.UpdateAsync(dealer);

            _logger.LogInformation("Dealer ID: {Id} is now {Status}", dealerId,
                dealer.IsActive ? "Active" : "Inactive");
        }

        public async Task<FarmerResponseDto> EditFarmerAsync(int farmerId, FarmerRegisterDto dto)
        {
            _logger.LogInformation("Admin editing farmer ID: {Id}", farmerId);

            var farmer = await _farmerRepo.GetByIdAsync(farmerId)
                ?? throw new KeyNotFoundException($"Farmer with ID {farmerId} not found.");

            farmer.Name = dto.Name;
            farmer.Phone = dto.Phone;
            farmer.Address = dto.Address;

            await _farmerRepo.UpdateAsync(farmer);
            _logger.LogInformation("Admin updated farmer ID: {Id}", farmerId);

            return new FarmerResponseDto
            {
                Id = farmer.Id,
                Name = farmer.Name,
                Email = farmer.Email,
                Phone = farmer.Phone
            };
        }

        public async Task<DealerResponseDto> EditDealerAsync(int dealerId, DealerRegisterDto dto)
        {
            _logger.LogInformation("Admin editing dealer ID: {Id}", dealerId);

            var dealer = await _dealerRepo.GetByIdAsync(dealerId)
                ?? throw new KeyNotFoundException($"Dealer with ID {dealerId} not found.");

            dealer.Name = dto.Name;
            dealer.Phone = dto.Phone;

            await _dealerRepo.UpdateAsync(dealer);
            _logger.LogInformation("Admin updated dealer ID: {Id}", dealerId);

            return new DealerResponseDto
            {
                Id = dealer.Id,
                Name = dealer.Name,
                Email = dealer.Email,
                Phone = dealer.Phone,
                IsActive = dealer.IsActive
            };
        }

        public async Task<DealerResponseDto> AddDealerAsync(DealerRegisterDto dto)
        {
            _logger.LogInformation("Admin adding new dealer with email: {Email}", dto.Email);

            var existing = await _dealerRepo.GetByEmailAsync(dto.Email);
            if (existing != null)
                throw new InvalidOperationException("A dealer with this email already exists.");

            var dealer = new Dealer
            {
                Name = dto.Name,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Phone = dto.Phone
            };

            var created = await _dealerRepo.CreateAsync(dealer);
            _logger.LogInformation("Admin created dealer ID: {Id}", created.Id);

            return new DealerResponseDto
            {
                Id = created.Id,
                Name = created.Name,
                Email = created.Email,
                Phone = created.Phone,
                IsActive = created.IsActive
            };
        }
    }
}
