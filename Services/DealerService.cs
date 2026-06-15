using CropDeals.Helpers;
using CropDeals.Models.DTOs;
using CropDeals.Models.Entities;
using CropDeals.Repositories.Interfaces;
using CropDeals.Services.Interfaces;

namespace CropDeals.Services
{
    public class DealerService : IDealerService
    {
        private readonly IDealerRepository _repo;
        private readonly JwtHelper _jwtHelper;
        private readonly ILogger<DealerService> _logger;

        public DealerService(IDealerRepository repo, JwtHelper jwtHelper, ILogger<DealerService> logger)
        {
            _repo = repo;
            _jwtHelper = jwtHelper;
            _logger = logger;
        }

        public async Task<DealerResponseDto> RegisterAsync(DealerRegisterDto dto)
        {
            _logger.LogInformation("Registration attempt for dealer email: {Email}", dto.Email);

            var existing = await _repo.GetByEmailAsync(dto.Email);
            if (existing != null)
            {
                _logger.LogWarning("Registration failed - email already exists: {Email}", dto.Email);
                throw new InvalidOperationException("A dealer with this email already exists.");
            }

            var dealer = new Dealer
            {
                Name = dto.Name,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Phone = dto.Phone
            };

            var created = await _repo.CreateAsync(dealer);
            _logger.LogInformation("Dealer registered successfully with ID: {Id}", created.Id);
            return MapToDto(created);
        }

        public async Task<string> LoginAsync(LoginDto dto)
        {
            _logger.LogInformation("Login attempt for dealer email: {Email}", dto.Email);

            var dealer = await _repo.GetByEmailAsync(dto.Email);
            if (dealer == null)
            {
                _logger.LogWarning("Login failed - email not found: {Email}", dto.Email);
                throw new UnauthorizedAccessException("Invalid email or password.");
            }

            if (!BCrypt.Net.BCrypt.Verify(dto.Password, dealer.PasswordHash))
            {
                _logger.LogWarning("Login failed - wrong password for email: {Email}", dto.Email);
                throw new UnauthorizedAccessException("Invalid email or password.");
            }

            _logger.LogInformation("Dealer logged in successfully. ID: {Id}", dealer.Id);
            return _jwtHelper.GenerateToken(dealer.Id, dealer.Email, "Dealer");
        }

        public async Task<DealerResponseDto> GetProfileAsync(int id)
        {
            _logger.LogInformation("Fetching profile for dealer ID: {Id}", id);

            var dealer = await _repo.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Dealer with ID {id} not found.");

            return MapToDto(dealer);
        }

        public async Task<DealerResponseDto> UpdateProfileAsync(int id, DealerRegisterDto dto)
        {
            _logger.LogInformation("Updating profile for dealer ID: {Id}", id);

            var dealer = await _repo.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Dealer with ID {id} not found.");

            dealer.Name = dto.Name;
            dealer.Phone = dto.Phone;

            var updated = await _repo.UpdateAsync(dealer);
            _logger.LogInformation("Profile updated successfully for dealer ID: {Id}", id);
            return MapToDto(updated);
        }

        public async Task<IEnumerable<DealerResponseDto>> GetAllAsync()
        {
            _logger.LogInformation("Fetching all dealers");
            var dealers = await _repo.GetAllAsync();
            return dealers.Select(MapToDto);
        }

        private static DealerResponseDto MapToDto(Dealer d) => new()
        {
            Id = d.Id,
            Name = d.Name,
            Email = d.Email,
            Phone = d.Phone,
            IsActive = d.IsActive
        };
    }
}