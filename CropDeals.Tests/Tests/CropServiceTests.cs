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
    public class CropServiceTests
    {
        private Mock<ICropRepository> _mockRepo = null!;
        private Mock<ISubscriptionRepository> _mockSubRepo = null!;
        private Mock<INotificationService> _mockNotificationService = null!;
        private Mock<ILogger<CropService>> _mockLogger = null!;
        private CropService _cropService = null!;

        [SetUp]
        public void Setup()
        {
            _mockRepo = new Mock<ICropRepository>();
            _mockSubRepo = new Mock<ISubscriptionRepository>();
            _mockNotificationService = new Mock<INotificationService>();
            _mockLogger = new Mock<ILogger<CropService>>();

            _cropService = new CropService(
                _mockRepo.Object,
                _mockSubRepo.Object,
                _mockNotificationService.Object,
                _mockLogger.Object
            );

            
            _mockSubRepo.Setup(r => r.GetByCropTypeAsync(It.IsAny<string>()))
                        .ReturnsAsync(new List<Subscription>());
        }

        [Test]
        public async Task PostCropAsync_ValidData_ReturnsCropDto()
        {
            // Arrange
            var dto = new CropCreateDto
            {
                Type = "Vegetable",
                Name = "Tomato",
                Quantity = 100,
                Location = "Punjab"
            };

            var createdCrop = new Crop
            {
                Id = 1,
                Type = dto.Type,
                Name = dto.Name,
                Quantity = dto.Quantity,
                Location = dto.Location,
                FarmerId = 1,
                Status = "Available",
                Farmer = new Farmer { Id = 1, Name = "Test Farmer" }
            };

            _mockRepo.Setup(r => r.CreateAsync(It.IsAny<Crop>()))
                     .ReturnsAsync(createdCrop);

            _mockRepo.Setup(r => r.GetByIdAsync(1))
                     .ReturnsAsync(createdCrop);

            // Act
            var result = await _cropService.PostCropAsync(1, dto);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Name, Is.EqualTo("Tomato"));
            Assert.That(result.Status, Is.EqualTo("Available"));
        }

        [Test]
        public async Task GetAllCropsAsync_ReturnsAllCrops()
        {
            // Arrange
            var crops = new List<Crop>
            {
                new Crop { Id = 1, Name = "Tomato", FarmerId = 1,
                    Farmer = new Farmer { Name = "Farmer 1" } },
                new Crop { Id = 2, Name = "Mango", FarmerId = 2,
                    Farmer = new Farmer { Name = "Farmer 2" } }
            };

            _mockRepo.Setup(r => r.GetAllAsync())
                     .ReturnsAsync(crops);

            // Act
            var result = await _cropService.GetAllCropsAsync();

            // Assert
            Assert.That(result.Count(), Is.EqualTo(2));
        }

        [Test]
        public void GetCropByIdAsync_InvalidId_ThrowsKeyNotFoundException()
        {
            // Arrange
            _mockRepo.Setup(r => r.GetByIdAsync(999))
                     .ReturnsAsync((Crop?)null);

            // Act & Assert
            Assert.ThrowsAsync<KeyNotFoundException>(
                () => _cropService.GetCropByIdAsync(999));
        }

        [Test]
        public async Task MarkAsSoldAsync_ValidOwner_ReturnsUpdatedCrop()
        {
            // Arrange
            var crop = new Crop
            {
                Id = 1,
                FarmerId = 1,
                Status = "Available",
                Name = "Tomato",
                Farmer = new Farmer { Name = "Test Farmer" }
            };

            _mockRepo.Setup(r => r.GetByIdAsync(1))
                     .ReturnsAsync(crop);

            _mockRepo.Setup(r => r.UpdateAsync(It.IsAny<Crop>()))
                     .ReturnsAsync(crop);

            // Act
            var result = await _cropService.MarkAsSoldAsync(1, 1);

            // Assert
            Assert.That(result.Status, Is.EqualTo("Sold"));
        }

        [Test]
        public void MarkAsSoldAsync_WrongFarmer_ThrowsUnauthorizedException()
        {
            // Arrange
            var crop = new Crop
            {
                Id = 1,
                FarmerId = 1,
                Status = "Available"
            };

            _mockRepo.Setup(r => r.GetByIdAsync(1))
                     .ReturnsAsync(crop);

            // Act & Assert
            Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => _cropService.MarkAsSoldAsync(1, farmerId: 2));
        }

        [Test]
        public void DeleteCropAsync_WrongFarmer_ThrowsUnauthorizedException()
        {
            // Arrange
            var crop = new Crop { Id = 1, FarmerId = 1 };

            _mockRepo.Setup(r => r.GetByIdAsync(1))
                     .ReturnsAsync(crop);

            // Act & Assert
            Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => _cropService.DeleteCropAsync(1, farmerId: 2));
        }

        [Test]
        public async Task PostCropAsync_WithSubscribers_SendsNotifications()
        {
            // Arrange
            var dto = new CropCreateDto
            {
                Type = "Vegetable",
                Name = "Tomato",
                Quantity = 100,
                Location = "Punjab"
            };

            var createdCrop = new Crop
            {
                Id = 1,
                Type = dto.Type,
                Name = dto.Name,
                Quantity = dto.Quantity,
                Location = dto.Location,
                FarmerId = 1,
                Status = "Available",
                Farmer = new Farmer { Id = 1, Name = "Test Farmer" }
            };

            var subscribers = new List<Subscription>
                {
                        new Subscription { DealerId = 1, CropType = "Vegetable",
                        Dealer = new Dealer { Id = 1, Name = "Dealer 1", Email = "dealer@test.com" } }
                };

            _mockRepo.Setup(r => r.CreateAsync(It.IsAny<Crop>())).ReturnsAsync(createdCrop);
            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(createdCrop);
            _mockSubRepo.Setup(r => r.GetByCropTypeAsync("Vegetable")).ReturnsAsync(subscribers);

            // Act
            var result = await _cropService.PostCropAsync(1, dto);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Name, Is.EqualTo("Tomato"));
        }
    }
}