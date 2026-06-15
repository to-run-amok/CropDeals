using CropDeals.Models.DTOs;
using CropDeals.Models.Entities;
using CropDeals.Repositories.Interfaces;
using CropDeals.Services;
using CropDeals.Tests;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace CropDeals.Tests
{
    [TestFixture]
    public class SubscriptionServiceTests
    {
        private Mock<ISubscriptionRepository> _mockRepo = null!;
        private Mock<ILogger<SubscriptionService>> _mockLogger = null!;
        private SubscriptionService _subscriptionService = null!;

        [SetUp]
        public void Setup()
        {
            _mockRepo = new Mock<ISubscriptionRepository>();
            _mockLogger = new Mock<ILogger<SubscriptionService>>();
            _subscriptionService = new SubscriptionService(_mockRepo.Object, _mockLogger.Object);
        }

        [Test]
        public async Task SubscribeAsync_ValidSubscription_ReturnsDto()
        {
            // Arrange
            var dto = new SubscriptionCreateDto { CropType = "Vegetable" };

            _mockRepo.Setup(r => r.GetAsync(1, "Vegetable"))
                     .ReturnsAsync((Subscription?)null);

            _mockRepo.Setup(r => r.CreateAsync(It.IsAny<Subscription>()))
                     .ReturnsAsync(new Subscription
                     {
                         Id = 1,
                         CropType = "Vegetable",
                         DealerId = 1
                     });

            // Act
            var result = await _subscriptionService.SubscribeAsync(1, dto);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.CropType, Is.EqualTo("Vegetable"));
        }

        [Test]
        public void SubscribeAsync_AlreadySubscribed_ThrowsInvalidOperationException()
        {
            // Arrange
            var dto = new SubscriptionCreateDto { CropType = "Vegetable" };

            _mockRepo.Setup(r => r.GetAsync(1, "Vegetable"))
                     .ReturnsAsync(new Subscription { CropType = "Vegetable" });

            // Act & Assert
            Assert.ThrowsAsync<InvalidOperationException>(
                () => _subscriptionService.SubscribeAsync(1, dto));
        }

        [Test]
        public async Task UnsubscribeAsync_ValidSubscription_DeletesSuccessfully()
        {
            // Arrange
            var subscription = new Subscription { Id = 1, CropType = "Vegetable", DealerId = 1 };

            _mockRepo.Setup(r => r.GetAsync(1, "Vegetable"))
                     .ReturnsAsync(subscription);

            _mockRepo.Setup(r => r.DeleteAsync(subscription))
                     .Returns(Task.CompletedTask);

            // Act & Assert — should not throw
            Assert.DoesNotThrowAsync(
                () => _subscriptionService.UnsubscribeAsync(1, "Vegetable"));
        }

        [Test]
        public void UnsubscribeAsync_NotSubscribed_ThrowsKeyNotFoundException()
        {
            // Arrange
            _mockRepo.Setup(r => r.GetAsync(1, "Fruit"))
                     .ReturnsAsync((Subscription?)null);

            // Act & Assert
            Assert.ThrowsAsync<KeyNotFoundException>(
                () => _subscriptionService.UnsubscribeAsync(1, "Fruit"));
        }

        [Test]
        public async Task GetMySubscriptionsAsync_ReturnsAllSubscriptions()
        {
            // Arrange
            var subs = new List<Subscription>
            {
                new Subscription { Id = 1, CropType = "Vegetable", DealerId = 1 },
                new Subscription { Id = 2, CropType = "Fruit", DealerId = 1 }
            };

            _mockRepo.Setup(r => r.GetByDealerIdAsync(1)).ReturnsAsync(subs);

            // Act
            var result = await _subscriptionService.GetMySubscriptionsAsync(1);

            // Assert
            Assert.That(result.Count(), Is.EqualTo(2));
        }
    }
}
