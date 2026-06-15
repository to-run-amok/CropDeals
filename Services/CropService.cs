using CropDeals.Models.DTOs;
using CropDeals.Models.Entities;
using CropDeals.Repositories.Interfaces;
using CropDeals.Services.Interfaces;

namespace CropDeals.Services
{
    public class CropService : ICropService
    {
        private readonly ICropRepository _repo;
        private readonly ILogger<CropService> _logger;
        private readonly ISubscriptionRepository _subscriptionRepo;
        private readonly INotificationService _notificationService;

        public CropService(ICropRepository repo,ISubscriptionRepository subscriptionRepo,INotificationService notificationService,ILogger<CropService> logger)
        {
            _repo = repo;
            _subscriptionRepo = subscriptionRepo;
            _notificationService = notificationService;
            _logger = logger;
        }

        public async Task<CropResponseDto> PostCropAsync(int farmerId, CropCreateDto dto)
        {
            _logger.LogInformation("Farmer ID: {FarmerId} posting crop: {CropName}", farmerId, dto.Name);

            var crop = new Crop
            {
                Type = dto.Type,
                Name = dto.Name,
                Quantity = dto.Quantity,
                Location = dto.Location,
                FarmerId = farmerId,
                Status = "Available"
            };

            var created = await _repo.CreateAsync(crop);
            var full = await _repo.GetByIdAsync(created.Id);
            _logger.LogInformation("Crop posted successfully with ID: {CropId}", created.Id);

            // notifing subscribed dealers
            var subscribers = await _subscriptionRepo.GetByCropTypeAsync(dto.Type);
            foreach (var sub in subscribers)
            {
                _ = Task.Run(() => _notificationService.SendCropNotificationAsync(
                    sub.Dealer.Email,
                    sub.Dealer.Name,
                    dto.Name,
                    dto.Type,
                    dto.Location,
                    dto.Quantity
                ));
            }

            if (subscribers.Any())
                _logger.LogInformation("Notified {Count} dealers about new crop", subscribers.Count());

            return MapToDto(full!);
        }

        public async Task<IEnumerable<CropResponseDto>> GetAllCropsAsync()
        {
            _logger.LogInformation("Fetching all crops");
            var crops = await _repo.GetAllAsync();
            return crops.Select(MapToDto);
        }

        public async Task<CropResponseDto> GetCropByIdAsync(int id)
        {
            _logger.LogInformation("Fetching crop with ID: {Id}", id);

            var crop = await _repo.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Crop with ID {id} not found.");

            return MapToDto(crop);
        }

        public async Task<IEnumerable<CropResponseDto>> GetMycropsAsync(int farmerId)
        {
            _logger.LogInformation("Fetching crops for farmer ID: {FarmerId}", farmerId);
            var crops = await _repo.GetByFarmerIdAsync(farmerId);
            return crops.Select(MapToDto);
        }

        public async Task<CropResponseDto> MarkAsSoldAsync(int cropId, int farmerId)
        {
            _logger.LogInformation("Farmer ID: {FarmerId} marking crop ID: {CropId} as sold", farmerId, cropId);

            var crop = await _repo.GetByIdAsync(cropId)
                ?? throw new KeyNotFoundException($"Crop with ID {cropId} not found.");

            if (crop.FarmerId != farmerId)
            {
                _logger.LogWarning("Unauthorized - Farmer ID: {FarmerId} tried to update crop ID: {CropId}", farmerId, cropId);
                throw new UnauthorizedAccessException("You can only update your own crops.");
            }

            crop.Status = "Sold";
            var updated = await _repo.UpdateAsync(crop);
            _logger.LogInformation("Crop ID: {CropId} marked as sold successfully", cropId);
            return MapToDto(updated);
        }

        public async Task DeleteCropAsync(int cropId, int farmerId)
        {
            _logger.LogInformation("Farmer ID: {FarmerId} deleting crop ID: {CropId}", farmerId, cropId);

            var crop = await _repo.GetByIdAsync(cropId)
                ?? throw new KeyNotFoundException($"Crop with ID {cropId} not found.");

            if (crop.FarmerId != farmerId)
            {
                _logger.LogWarning("Unauthorized - Farmer ID: {FarmerId} tried to delete crop ID: {CropId}", farmerId, cropId);
                throw new UnauthorizedAccessException("You can only delete your own crops.");
            }

            await _repo.DeleteAsync(crop);
            _logger.LogInformation("Crop ID: {CropId} deleted successfully", cropId);
        }

        private static CropResponseDto MapToDto(Crop c) => new()
        {
            Id = c.Id,
            Type = c.Type,
            Name = c.Name,
            Quantity = c.Quantity,
            Location = c.Location,
            Status = c.Status,
            PostedAt = c.PostedAt,
            FarmerId = c.FarmerId,
            FarmerName = c.Farmer?.Name ?? "Unknown"
        };


    }
}