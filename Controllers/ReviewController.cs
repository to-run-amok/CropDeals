using CropDeals.Models.DTOs;
using CropDeals.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CropDeals.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        // POST /api/Review 
        [HttpPost]
        [Authorize(Roles = "Dealer")] // Dealer can review farmer after purchase
        public async Task<IActionResult> AddReview([FromBody] ReviewCreateDto dto)
        {
            var dealerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _reviewService.AddReviewAsync(dealerId, dto);
            return Ok(result);
        }

        // GET /api/Review/farmer/{id} 
        [HttpGet("farmer/{id}")]
        [Authorize] //Anyone can see a farmers reviews
        public async Task<IActionResult> GetFarmerReviews(int id)
        {
            var result = await _reviewService.GetFarmerReviewsAsync(id);
            return Ok(result);
        }

        // GET /api/Review  
        [HttpGet]
        [Authorize(Roles = "Admin")]    //Admin sees all reviews
        public async Task<IActionResult> GetAllReviews()
        {
            var result = await _reviewService.GetAllReviewsAsync();
            return Ok(result);
        }
    }
}