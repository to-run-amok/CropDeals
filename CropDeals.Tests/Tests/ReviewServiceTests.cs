using CropDeals.Models.DTOs;
using CropDeals.Models.Entities;
using CropDeals.Repositories.Interfaces;
using CropDeals.Services;
using CropDeals.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace CropDeals.Tests
{
    [TestFixture]
    public class ReviewServiceTests
    {
        private Mock<IReviewRepository> _mockReviewRepo = null!;
        private Mock<ICropRepository> _mockCropRepo = null!;
        private Mock<IFarmerRepository> _mockFarmerRepo = null!;
        private Mock<IDealerRepository> _mockDealerRepo = null!;
        private Mock<ILogger<ReviewService>> _mockLogger = null!;
        private ReviewService _reviewService = null!;

        [SetUp]
        public void Setup()
        {
            _mockReviewRepo = new Mock<IReviewRepository>();
            _mockCropRepo = new Mock<ICropRepository>();
            _mockFarmerRepo = new Mock<IFarmerRepository>();
            _mockDealerRepo = new Mock<IDealerRepository>();
            _mockLogger = new Mock<ILogger<ReviewService>>();

            _reviewService = new ReviewService(
                _mockReviewRepo.Object,
                _mockCropRepo.Object,
                _mockFarmerRepo.Object,
                _mockDealerRepo.Object,
                _mockLogger.Object
            );
        }

        [Test]
        public async Task AddReviewAsync_ValidReview_ReturnsReviewDto()
        {
            // Arrange
            var dto = new ReviewCreateDto
            {
                FarmerId = 1,
                CropId = 1,
                Rating = 5,
                Comment = "Excellent quality!"
            };

            var crop = new Crop { Id = 1, Name = "Tomato", PurchasedByDealerId = 1 };
            var farmer = new Farmer { Id = 1, Name = "Test Farmer" };
            var dealer = new Dealer { Id = 1, Name = "Test Dealer" };

            _mockCropRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(crop);
            _mockFarmerRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(farmer);
            _mockDealerRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(dealer);
            _mockReviewRepo.Setup(r => r.ExistsAsync(1, 1)).ReturnsAsync(false);
            _mockReviewRepo.Setup(r => r.CreateAsync(It.IsAny<Review>()))
                           .ReturnsAsync(new Review
                           {
                               Id = 1,
                               Rating = dto.Rating,
                               Comment = dto.Comment,
                               Dealer = dealer,
                               Farmer = farmer,
                               Crop = crop
                           });

            // Act
            var result = await _reviewService.AddReviewAsync(1, dto);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Rating, Is.EqualTo(5));
            Assert.That(result.Comment, Is.EqualTo("Excellent quality!"));
        }

        [Test]
        public void AddReviewAsync_InvalidRating_ThrowsInvalidOperationException()
        {
            // Arrange — rating of 6 is invalid
            var dto = new ReviewCreateDto
            {
                FarmerId = 1,
                CropId = 1,
                Rating = 6,
                Comment = "Test"
            };

            // Act & Assert
            Assert.ThrowsAsync<InvalidOperationException>(
                () => _reviewService.AddReviewAsync(1, dto));
        }

        [Test]
        public void AddReviewAsync_NotPurchasedByDealer_ThrowsUnauthorizedException()
        {
            // Arrange — crop was purchased by dealer 2 but dealer 1 is reviewing
            var crop = new Crop { Id = 1, PurchasedByDealerId = 2 };

            _mockCropRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(crop);

            var dto = new ReviewCreateDto { FarmerId = 1, CropId = 1, Rating = 4 };

            // Act & Assert
            Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => _reviewService.AddReviewAsync(1, dto));
        }

        [Test]
        public void AddReviewAsync_DuplicateReview_ThrowsInvalidOperationException()
        {
            // Arrange
            var crop = new Crop { Id = 1, PurchasedByDealerId = 1 };
            var dealer = new Dealer { Id = 1, Name = "Test Dealer" };

            _mockCropRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(crop);
            _mockDealerRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(dealer);
            _mockReviewRepo.Setup(r => r.ExistsAsync(1, 1)).ReturnsAsync(true);

            var dto = new ReviewCreateDto { FarmerId = 1, CropId = 1, Rating = 4 };

            // Act & Assert
            Assert.ThrowsAsync<InvalidOperationException>(
                () => _reviewService.AddReviewAsync(1, dto));
        }

        [Test]
        public void AddReviewAsync_CropNotFound_ThrowsKeyNotFoundException()
        {
            // Arrange
            _mockCropRepo.Setup(r => r.GetByIdAsync(999))
                         .ReturnsAsync((Crop?)null);

            var dto = new ReviewCreateDto { CropId = 999, Rating = 4 };

            // Act & Assert
            Assert.ThrowsAsync<KeyNotFoundException>(
                () => _reviewService.AddReviewAsync(1, dto));
        }
    }
}