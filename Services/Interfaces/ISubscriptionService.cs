using CropDeals.Models.DTOs;

namespace CropDeals.Services.Interfaces
{
    public interface ISubscriptionService
    {
        Task<SubscriptionResponseDto> SubscribeAsync(int dealerId, SubscriptionCreateDto dto);
        Task UnsubscribeAsync(int dealerId, string cropType);
        Task<IEnumerable<SubscriptionResponseDto>> GetMySubscriptionsAsync(int dealerId);
    }
}