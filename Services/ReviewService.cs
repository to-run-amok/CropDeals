using CropDeals.Models.DTOs;
using CropDeals.Models.Entities;
using CropDeals.Repositories.Interfaces;
using CropDeals.Services.Interfaces;

namespace CropDeals.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepo;
        private readonly ICropRepository _cropRepo;
        private readonly IFarmerRepository _farmerRepo;
        private readonly IDealerRepository _dealerRepo;
        private readonly ILogger<ReviewService> _logger;

        public ReviewService(
            IReviewRepository reviewRepo,
            ICropRepository cropRepo,
            IFarmerRepository farmerRepo,
            IDealerRepository dealerRepo,
            ILogger<ReviewService> logger)
        {
            _reviewRepo = reviewRepo;
            _cropRepo = cropRepo;
            _farmerRepo = farmerRepo;
            _dealerRepo = dealerRepo;
            _logger = logger;
        }

        public async Task<ReviewResponseDto> AddReviewAsync(int dealerId, ReviewCreateDto dto)
        {
            _logger.LogInformation("Dealer ID: {DealerId} adding review for farmer ID: {FarmerId}", dealerId, dto.FarmerId);

            
            if (dto.Rating < 1 || dto.Rating > 5)
                throw new InvalidOperationException("Rating must be between 1 and 5.");

           
            var crop = await _cropRepo.GetByIdAsync(dto.CropId)
                ?? throw new KeyNotFoundException($"Crop with ID {dto.CropId} not found.");

            if (crop.PurchasedByDealerId != dealerId)
                throw new UnauthorizedAccessException("You can only review farmers whose crops you have purchased.");

          
            var alreadyReviewed = await _reviewRepo.ExistsAsync(dealerId, dto.CropId);
            if (alreadyReviewed)
                throw new InvalidOperationException("You have already reviewed this purchase.");

            var dealer = await _dealerRepo.GetByIdAsync(dealerId)
                ?? throw new KeyNotFoundException("Dealer not found.");

            var farmer = await _farmerRepo.GetByIdAsync(dto.FarmerId)
                ?? throw new KeyNotFoundException("Farmer not found.");

            var review = new Review
            {
                Rating = dto.Rating,
                Comment = dto.Comment,
                DealerId = dealerId,
                FarmerId = dto.FarmerId,
                CropId = dto.CropId
            };

            var created = await _reviewRepo.CreateAsync(review);
            _logger.LogInformation("Review added successfully ID: {Id}", created.Id);

            return new ReviewResponseDto
            {
                Id = created.Id,
                Rating = created.Rating,
                Comment = created.Comment,
                DealerName = dealer.Name,
                FarmerName = farmer.Name,
                CropName = crop.Name,
                CreatedAt = created.CreatedAt
            };
        }

        public async Task<IEnumerable<ReviewResponseDto>> GetFarmerReviewsAsync(int farmerId)
        {
            _logger.LogInformation("Fetching reviews for farmer ID: {FarmerId}", farmerId);
            var reviews = await _reviewRepo.GetByFarmerIdAsync(farmerId);
            return reviews.Select(MapToDto);
        }

        public async Task<IEnumerable<ReviewResponseDto>> GetAllReviewsAsync()
        {
            _logger.LogInformation("Admin fetching all reviews");
            var reviews = await _reviewRepo.GetAllAsync();
            return reviews.Select(MapToDto);
        }

        private static ReviewResponseDto MapToDto(Review r) => new()
        {
            Id = r.Id,
            Rating = r.Rating,
            Comment = r.Comment,
            DealerName = r.Dealer.Name,
            FarmerName = r.Farmer.Name,
            CropName = r.Crop.Name,
            CreatedAt = r.CreatedAt
        };
    }
}