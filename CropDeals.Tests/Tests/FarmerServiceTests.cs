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
    public class FarmerServiceTests
    {
        private Mock<IFarmerRepository> _mockRepo = null!;
        private FarmerService _farmerService = null!;
        private JwtHelper _jwtHelper = null!;
        private Mock<ILogger<FarmerService>> _mockLogger = null!;

        [SetUp]
        public void Setup()
        {
            _mockRepo = new Mock<IFarmerRepository>();
            _mockLogger = new Mock<ILogger<FarmerService>>();

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
            _farmerService = new FarmerService(_mockRepo.Object, _jwtHelper, _mockLogger.Object);
        }

        // ─── REGISTER TESTS ──────────────────────────────────

        [Test]
        public async Task RegisterAsync_ValidData_ReturnsFarmerDto()
        {
            // Arrange
            var dto = new FarmerRegisterDto
            {
                Name = "Test Farmer",
                Email = "farmer@test.com",
                Password = "password123",
                Phone = "9999999999",
                Address = "Farm Lane"
            };

            _mockRepo.Setup(r => r.GetByEmailAsync(dto.Email))
                     .ReturnsAsync((Farmer?)null);

            _mockRepo.Setup(r => r.CreateAsync(It.IsAny<Farmer>()))
                     .ReturnsAsync(new Farmer
                     {
                         Id = 1,
                         Name = dto.Name,
                         Email = dto.Email,
                         PasswordHash = "hashed",
                         Phone = dto.Phone
                     });

            // Act
            var result = await _farmerService.RegisterAsync(dto);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Email, Is.EqualTo(dto.Email));
            Assert.That(result.Name, Is.EqualTo(dto.Name));
        }

        [Test]
        public void RegisterAsync_DuplicateEmail_ThrowsInvalidOperationException()
        {
            // Arrange
            var dto = new FarmerRegisterDto
            {
                Email = "taken@farm.com",
                Password = "pass"
            };

            _mockRepo.Setup(r => r.GetByEmailAsync(dto.Email))
                     .ReturnsAsync(new Farmer { Email = dto.Email });

            // Act & Assert
            Assert.ThrowsAsync<InvalidOperationException>(
                () => _farmerService.RegisterAsync(dto));
        }

        [Test]
        public async Task RegisterAsync_PasswordIsHashed_NotStoredAsPlainText()
        {
            // Arrange
            var dto = new FarmerRegisterDto
            {
                Name = "Farmer",
                Email = "hash@test.com",
                Password = "plainpassword"
            };

            Farmer? savedFarmer = null;

            _mockRepo.Setup(r => r.GetByEmailAsync(dto.Email))
                     .ReturnsAsync((Farmer?)null);

            _mockRepo.Setup(r => r.CreateAsync(It.IsAny<Farmer>()))
                     .Callback<Farmer>(f => savedFarmer = f)
                     .ReturnsAsync((Farmer f) => f);

            // Act
            await _farmerService.RegisterAsync(dto);

            // Assert — password should never be stored as plain text
            Assert.That(savedFarmer, Is.Not.Null);
            Assert.That(savedFarmer!.PasswordHash, Is.Not.EqualTo(dto.Password));
        }

        // ─── LOGIN TESTS ─────────────────────────────────────

        [Test]
        public async Task LoginAsync_ValidCredentials_ReturnsToken()
        {
            // Arrange
            var password = "mypassword";

            _mockRepo.Setup(r => r.GetByEmailAsync("farmer@test.com"))
                     .ReturnsAsync(new Farmer
                     {
                         Id = 1,
                         Email = "farmer@test.com",
                         PasswordHash = BCrypt.Net.BCrypt.HashPassword(password)
                     });

            // Act
            var token = await _farmerService.LoginAsync(new LoginDto
            {
                Email = "farmer@test.com",
                Password = password
            });

            // Assert
            Assert.That(token, Is.Not.Null.And.Not.Empty);
        }

        [Test]
        public void LoginAsync_WrongPassword_ThrowsUnauthorizedException()
        {
            // Arrange
            _mockRepo.Setup(r => r.GetByEmailAsync("farmer@test.com"))
                     .ReturnsAsync(new Farmer
                     {
                         Id = 1,
                         Email = "farmer@test.com",
                         PasswordHash = BCrypt.Net.BCrypt.HashPassword("correctpassword")
                     });

            // Act & Assert
            Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => _farmerService.LoginAsync(new LoginDto
                {
                    Email = "farmer@test.com",
                    Password = "wrongpassword"
                }));
        }

        [Test]
        public void LoginAsync_EmailNotFound_ThrowsUnauthorizedException()
        {
            // Arrange
            _mockRepo.Setup(r => r.GetByEmailAsync("nobody@test.com"))
                     .ReturnsAsync((Farmer?)null);

            // Act & Assert
            Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => _farmerService.LoginAsync(new LoginDto
                {
                    Email = "nobody@test.com",
                    Password = "anypassword"
                }));
        }

        // ─── PROFILE TESTS ───────────────────────────────────

        [Test]
        public async Task GetProfileAsync_ValidId_ReturnsFarmerDto()
        {
            // Arrange
            _mockRepo.Setup(r => r.GetByIdAsync(1))
                     .ReturnsAsync(new Farmer
                     {
                         Id = 1,
                         Name = "Test Farmer",
                         Email = "farmer@test.com",
                         Phone = "9999999999"
                     });

            // Act
            var result = await _farmerService.GetProfileAsync(1);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(1));
            Assert.That(result.Name, Is.EqualTo("Test Farmer"));
        }

        [Test]
        public void GetProfileAsync_InvalidId_ThrowsKeyNotFoundException()
        {
            // Arrange
            _mockRepo.Setup(r => r.GetByIdAsync(999))
                     .ReturnsAsync((Farmer?)null);

            // Act & Assert
            Assert.ThrowsAsync<KeyNotFoundException>(
                () => _farmerService.GetProfileAsync(999));
        }
    }
}