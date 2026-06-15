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
    public class PaymentServiceTests
    {
        private Mock<IPaymentRepository> _mockPaymentRepo = null!;
        private Mock<ICropRepository> _mockCropRepo = null!;
        private Mock<IFarmerRepository> _mockFarmerRepo = null!;
        private Mock<IDealerRepository> _mockDealerRepo = null!;
        private Mock<ILogger<PaymentService>> _mockLogger = null!;
        private PaymentService _paymentService = null!;

        [SetUp]
        public void Setup()
        {
            _mockPaymentRepo = new Mock<IPaymentRepository>();
            _mockCropRepo = new Mock<ICropRepository>();
            _mockFarmerRepo = new Mock<IFarmerRepository>();
            _mockDealerRepo = new Mock<IDealerRepository>();
            _mockLogger = new Mock<ILogger<PaymentService>>();

            _paymentService = new PaymentService(
                _mockPaymentRepo.Object,
                _mockCropRepo.Object,
                _mockFarmerRepo.Object,
                _mockDealerRepo.Object,
                _mockLogger.Object
            );
        }

        [Test]
        public async Task MakePaymentAsync_ValidPayment_ReturnsPaymentResponse()
        {
            // Arrange
            var dto = new PaymentRequestDto
            {
                CropId = 1,
                Amount = 5000,
                PaymentMethod = "CreditCard",
                CardLastFourDigits = "1234"
            };

            var crop = new Crop
            {
                Id = 1,
                Name = "Tomato",
                Status = "Sold",
                FarmerId = 1,
                PurchasedByDealerId = 1
            };

            var farmer = new Farmer { Id = 1, Name = "Test Farmer" };
            var dealer = new Dealer { Id = 1, Name = "Test Dealer" };

            _mockCropRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(crop);
            _mockFarmerRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(farmer);
            _mockDealerRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(dealer);
            _mockPaymentRepo.Setup(r => r.GetByDealerIdAsync(1))
                            .ReturnsAsync(new List<Payment>());
            _mockPaymentRepo.Setup(r => r.CreateAsync(It.IsAny<Payment>()))
                            .ReturnsAsync((Payment p) => p);

            // Act
            var result = await _paymentService.MakePaymentAsync(1, dto);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Status, Is.EqualTo("Success"));
            Assert.That(result.Amount, Is.EqualTo(5000));
            Assert.That(result.TransactionId, Does.StartWith("TXN-"));
        }

        [Test]
        public void MakePaymentAsync_CropNotSold_ThrowsInvalidOperationException()
        {
            // Arrange
            var crop = new Crop
            {
                Id = 1,
                Status = "Available",
                PurchasedByDealerId = 1
            };

            _mockCropRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(crop);

            // Act & Assert
            Assert.ThrowsAsync<InvalidOperationException>(
                () => _paymentService.MakePaymentAsync(1, new PaymentRequestDto { CropId = 1 }));
        }

        [Test]
        public void MakePaymentAsync_WrongDealer_ThrowsUnauthorizedException()
        {
            // Arrange — crop was purchased by dealer 1 but dealer 2 is trying to pay
            var crop = new Crop
            {
                Id = 1,
                Status = "Sold",
                PurchasedByDealerId = 1
            };

            _mockCropRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(crop);

            // Act & Assert
            Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => _paymentService.MakePaymentAsync(2, new PaymentRequestDto { CropId = 1 }));
        }

        [Test]
        public void MakePaymentAsync_AlreadyPaid_ThrowsInvalidOperationException()
        {
            // Arrange
            var crop = new Crop
            {
                Id = 1,
                Status = "Sold",
                FarmerId = 1,
                PurchasedByDealerId = 1
            };

            // Payment already exists for this crop
            var existingPayments = new List<Payment>
            {
                new Payment { CropId = 1, DealerId = 1 }
            };

            _mockCropRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(crop);
            _mockPaymentRepo.Setup(r => r.GetByDealerIdAsync(1))
                            .ReturnsAsync(existingPayments);

            // Act & Assert
            Assert.ThrowsAsync<InvalidOperationException>(
                () => _paymentService.MakePaymentAsync(1, new PaymentRequestDto { CropId = 1 }));
        }

        [Test]
        public void MakePaymentAsync_CropNotFound_ThrowsKeyNotFoundException()
        {
            // Arrange
            _mockCropRepo.Setup(r => r.GetByIdAsync(999))
                         .ReturnsAsync((Crop?)null);

            // Act & Assert
            Assert.ThrowsAsync<KeyNotFoundException>(
                () => _paymentService.MakePaymentAsync(1, new PaymentRequestDto { CropId = 999 }));
        }
    }
}