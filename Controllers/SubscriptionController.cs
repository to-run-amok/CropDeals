using CropDeals.Models.DTOs;
using CropDeals.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CropDeals.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SubscriptionController : ControllerBase
    {
        private readonly ISubscriptionService _subscriptionService;

        public SubscriptionController(ISubscriptionService subscriptionService)
        {
            _subscriptionService = subscriptionService;
        }

        // POST /api/Subscription  
        [HttpPost]
        [Authorize(Roles = "Dealer")] //only Dealer can subscribe to a crop type
        public async Task<IActionResult> Subscribe([FromBody] SubscriptionCreateDto dto)
        {
            var dealerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _subscriptionService.SubscribeAsync(dealerId, dto);
            return Ok(result);
        }

        // DELETE /api/Subscription/{cropType} 
        [HttpDelete("{cropType}")]
        [Authorize(Roles = "Dealer")] //only Dealer can unsubscribe
        public async Task<IActionResult> Unsubscribe(string cropType)
        {
            var dealerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await _subscriptionService.UnsubscribeAsync(dealerId, cropType);
            return Ok(new { Message = $"Unsubscribed from {cropType} notifications." });
        }

        // GET /api/Subscription 
        [HttpGet]
        [Authorize(Roles = "Dealer")] //Dealer can sees their subscriptions
        public async Task<IActionResult> GetMySubscriptions()
        {
            var dealerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _subscriptionService.GetMySubscriptionsAsync(dealerId);
            return Ok(result);
        }
    }
}