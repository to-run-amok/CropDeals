using CropDeals.Helpers;
using CropDeals.Models.DTOs;
using CropDeals.Models.Entities;
using CropDeals.Repositories.Interfaces;
using CropDeals.Services.Interfaces;

namespace CropDeals.Services
{
    public class FarmerService : IFarmerService
    {
        private readonly IFarmerRepository _repo;
        private readonly JwtHelper _jwtHelper;
        private readonly ILogger<FarmerService> _logger;

        public FarmerService(IFarmerRepository repo, JwtHelper jwtHelper, ILogger<FarmerService> logger)
        {
            _repo = repo;
            _jwtHelper = jwtHelper;
            _logger = logger;
        }

        public async Task<FarmerResponseDto> RegisterAsync(FarmerRegisterDto dto)
        {
            _logger.LogInformation("Registration attempt for farmer email: {Email}", dto.Email);

            var existing = await _repo.GetByEmailAsync(dto.Email);
            if (existing != null)
            {
                _logger.LogWarning("Registration failed - email already exists: {Email}", dto.Email);
                throw new InvalidOperationException("A farmer with this email already exists.");
            }

            var farmer = new Farmer
            {
                Name = dto.Name,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Phone = dto.Phone,
                Address = dto.Address
            };

            var created = await _repo.CreateAsync(farmer);
            _logger.LogInformation("Farmer registered successfully with ID: {Id}", created.Id);
            return MapToDto(created);
        }

        public async Task<string> LoginAsync(LoginDto dto)
        {
            _logger.LogInformation("Login attempt for farmer email: {Email}", dto.Email);

            var farmer = await _repo.GetByEmailAsync(dto.Email);
            if (farmer == null)
            {
                _logger.LogWarning("Login failed - email not found: {Email}", dto.Email);
                throw new UnauthorizedAccessException("Invalid email or password.");
            }

            if (!BCrypt.Net.BCrypt.Verify(dto.Password, farmer.PasswordHash))
            {
                _logger.LogWarning("Login failed - wrong password for email: {Email}", dto.Email);
                throw new UnauthorizedAccessException("Invalid email or password.");
            }

            _logger.LogInformation("Farmer logged in successfully. ID: {Id}", farmer.Id);
            return _jwtHelper.GenerateToken(farmer.Id, farmer.Email, "Farmer");
        }

        public async Task<FarmerResponseDto> GetProfileAsync(int id)
        {
            _logger.LogInformation("Fetching profile for farmer ID: {Id}", id);

            var farmer = await _repo.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Farmer with ID {id} not found.");

            return MapToDto(farmer);
        }

        public async Task<FarmerResponseDto> UpdateProfileAsync(int id, FarmerRegisterDto dto)
        {
            _logger.LogInformation("Updating profile for farmer ID: {Id}", id);

            var farmer = await _repo.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Farmer with ID {id} not found.");

            farmer.Name = dto.Name;
            farmer.Phone = dto.Phone;
            farmer.Address = dto.Address;

            var updated = await _repo.UpdateAsync(farmer);
            _logger.LogInformation("Profile updated successfully for farmer ID: {Id}", id);
            return MapToDto(updated);
        }

        private static FarmerResponseDto MapToDto(Farmer f) => new()
        {
            Id = f.Id,
            Name = f.Name,
            Email = f.Email,
            Phone = f.Phone
        };
    }
}