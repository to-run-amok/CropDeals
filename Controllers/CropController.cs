using CropDeals.Models.DTOs;
using CropDeals.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CropDeals.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CropController : ControllerBase
    {
        private readonly ICropService _cropService;

        public CropController(ICropService cropService)
        {
            _cropService = cropService;
        }

        // POST /api/Crop 
        [HttpPost]
        [Authorize(Roles = "Farmer")]   //Only Farmers can post crops
        public async Task<IActionResult> PostCrop([FromBody] CropCreateDto dto)
        {
            var farmerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _cropService.PostCropAsync(farmerId, dto);
            return CreatedAtAction(nameof(GetCrop), new { id = result.Id }, result);
        }

        // GET /api/Crop 
        [HttpGet]
        [Authorize] //anyone with valid token can access this
        public async Task<IActionResult> GetAllCrops()
        {
            var crops = await _cropService.GetAllCropsAsync();
            return Ok(crops);
        }

        // GET /api/Crop/{id} 
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetCrop(int id)
        {
            var crop = await _cropService.GetCropByIdAsync(id);
            return Ok(crop);
        }

        // GET /api/Crop/my-crops 
        [HttpGet("my-crops")]
        [Authorize(Roles = "Farmer")] //only said farmer can view their own crop
        public async Task<IActionResult> GetMyCrops()
        {
            var farmerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var crops = await _cropService.GetMycropsAsync(farmerId);
            return Ok(crops);
        }

        // PUT /api/Crop/{id}/mark-sold 
        [HttpPut("{id}/mark-sold")]
        [Authorize(Roles = "Farmer")] //only said farmer can mark their own crop as sold
        public async Task<IActionResult> MarkAsSold(int id)
        {
            var farmerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _cropService.MarkAsSoldAsync(id, farmerId);
            return Ok(result);
        }

        // DELETE /api/Crop/{id} 
        [HttpDelete("{id}")]
        [Authorize(Roles = "Farmer")] //only said farmer can deleted their own crop
        public async Task<IActionResult> DeleteCrop(int id)
        {
            var farmerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await _cropService.DeleteCropAsync(id, farmerId);
            return NoContent();
        }
    }
}