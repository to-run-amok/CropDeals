using CropDeals.Models.DTOs;
using CropDeals.Models.Entities;
using CropDeals.Repositories.Interfaces;
using CropDeals.Services.Interfaces;

namespace CropDeals.Services
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly ISubscriptionRepository _repo;
        private readonly ILogger<SubscriptionService> _logger;

        public SubscriptionService(ISubscriptionRepository repo, ILogger<SubscriptionService> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        public async Task<SubscriptionResponseDto> SubscribeAsync(int dealerId, SubscriptionCreateDto dto)
        {
            _logger.LogInformation("Dealer ID: {DealerId} subscribing to: {CropType}", dealerId, dto.CropType);

            
            var existing = await _repo.GetAsync(dealerId, dto.CropType);
            if (existing != null)
                throw new InvalidOperationException($"You are already subscribed to {dto.CropType} notifications.");

            var subscription = new Subscription
            {
                DealerId = dealerId,
                CropType = dto.CropType
            };

            var created = await _repo.CreateAsync(subscription);
            _logger.LogInformation("Subscription created ID: {Id}", created.Id);

            return new SubscriptionResponseDto
            {
                Id = created.Id,
                CropType = created.CropType,
                SubscribedAt = created.SubscribedAt
            };
        }

        public async Task UnsubscribeAsync(int dealerId, string cropType)
        {
            _logger.LogInformation("Dealer ID: {DealerId} unsubscribing from: {CropType}", dealerId, cropType);

            var subscription = await _repo.GetAsync(dealerId, cropType)
                ?? throw new KeyNotFoundException($"You are not subscribed to {cropType} notifications.");

            await _repo.DeleteAsync(subscription);
            _logger.LogInformation("Dealer ID: {DealerId} unsubscribed from: {CropType}", dealerId, cropType);
        }

        public async Task<IEnumerable<SubscriptionResponseDto>> GetMySubscriptionsAsync(int dealerId)
        {
            var subs = await _repo.GetByDealerIdAsync(dealerId);
            return subs.Select(s => new SubscriptionResponseDto
            {
                Id = s.Id,
                CropType = s.CropType,
                SubscribedAt = s.SubscribedAt
            });
        }
    }
}