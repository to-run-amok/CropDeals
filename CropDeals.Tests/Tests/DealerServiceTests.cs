using CropDeals.Helpers;
using CropDeals.Models.DTOs;
using CropDeals.Models.Entities;
using CropDeals.Repositories.Interfaces;
using CropDeals.Services;
using Microsoft.Extensions.Configuration;
using Moq;
using Microsoft.Extensions.Logging;
using NUnit.Framework;

namespace CropDeals.Tests
{
    [TestFixture]
    public class DealerServiceTests
    {
        private Mock<IDealerRepository> _mockRepo = null!;
        private DealerService _dealerService = null!;
        private JwtHelper _jwtHelper = null!;
        private Mock<ILogger<DealerService>> _mockLogger = null!;

        [SetUp]
        public void Setup()
        {
            _mockRepo = new Mock<IDealerRepository>();
            _mockLogger = new Mock<ILogger<DealerService>>();

            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    { "Jwt:Key", "TestSecretKeyThatIsAtLeast32CharactersLong!" },
                    { "Jwt:Issuer", "CropDealTest" },
                    { "Jwt:Audience", "CropDealTestUsers" },
                    { "Jwt:ExpiryMinutes", "60" }
                })
                .Build();

            _jwtHelper = new JwtHelper(config);
            _dealerService = new DealerService(_mockRepo.Object, _jwtHelper, _mockLogger.Object);
        }

        [Test]
        public async Task RegisterAsync_ValidData_ReturnsDealerDto()
        {
            // Arrange
            var dto = new DealerRegisterDto
            {
                Name = "Test Dealer",
                Email = "dealer@test.com",
                Password = "password123",
                Phone = "8888888888"
            };

            _mockRepo.Setup(r => r.GetByEmailAsync(dto.Email))
                     .ReturnsAsync((Dealer?)null);

            _mockRepo.Setup(r => r.CreateAsync(It.IsAny<Dealer>()))
                     .ReturnsAsync(new Dealer
                     {
                         Id = 1,
                         Name = dto.Name,
                         Email = dto.Email,
                         Phone = dto.Phone
                     });

            // Act
            var result = await _dealerService.RegisterAsync(dto);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Email, Is.EqualTo(dto.Email));
        }

        [Test]
        public void RegisterAsync_DuplicateEmail_ThrowsInvalidOperationException()
        {
            // Arrange
            var dto = new DealerRegisterDto { Email = "taken@dealer.com", Password = "pass" };

            _mockRepo.Setup(r => r.GetByEmailAsync(dto.Email))
                     .ReturnsAsync(new Dealer { Email = dto.Email });

            // Act & Assert
            Assert.ThrowsAsync<InvalidOperationException>(
                () => _dealerService.RegisterAsync(dto));
        }

        [Test]
        public async Task LoginAsync_ValidCredentials_ReturnsToken()
        {
            // Arrange
            var password = "dealerpass";

            _mockRepo.Setup(r => r.GetByEmailAsync("dealer@test.com"))
                     .ReturnsAsync(new Dealer
                     {
                         Id = 1,
                         Email = "dealer@test.com",
                         PasswordHash = BCrypt.Net.BCrypt.HashPassword(password)
                     });

            // Act
            var token = await _dealerService.LoginAsync(new LoginDto
            {
                Email = "dealer@test.com",
                Password = password
            });

            // Assert
            Assert.That(token, Is.Not.Null.And.Not.Empty);
        }

        [Test]
        public void LoginAsync_WrongPassword_ThrowsUnauthorizedException()
        {
            // Arrange
            _mockRepo.Setup(r => r.GetByEmailAsync("dealer@test.com"))
                     .ReturnsAsync(new Dealer
                     {
                         Id = 1,
                         Email = "dealer@test.com",
                         PasswordHash = BCrypt.Net.BCrypt.HashPassword("rightpassword")
                     });

            // Act & Assert
            Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => _dealerService.LoginAsync(new LoginDto
                {
                    Email = "dealer@test.com",
                    Password = "wrongpassword"
                }));
        }

        [Test]
        public void GetProfileAsync_InvalidId_ThrowsKeyNotFoundException()
        {
            // Arrange
            _mockRepo.Setup(r => r.GetByIdAsync(999))
                     .ReturnsAsync((Dealer?)null);

            // Act & Assert
            Assert.ThrowsAsync<KeyNotFoundException>(
                () => _dealerService.GetProfileAsync(999));
        }
    }
}