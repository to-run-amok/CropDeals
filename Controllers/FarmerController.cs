using CropDeals.Models.DTOs;
using CropDeals.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CropDeals.Controllers
{
    [ApiController]
    [Route("api/[controller]")]   // URL: /api/Farmer
    public class FarmerController : ControllerBase
    {
        private readonly IFarmerService _farmerService;

        public FarmerController(IFarmerService farmerService)
        {
            _farmerService = farmerService;
        }

        // POST /api/Farmer/register — Anyone can register (no token needed)
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] FarmerRegisterDto dto)
        {
            var result = await _farmerService.RegisterAsync(dto);
            return CreatedAtAction(nameof(GetProfile), new { id = result.Id }, result);
        }

        // POST /api/Farmer/login — Returns JWT token
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var token = await _farmerService.LoginAsync(dto);
            return Ok(new { Token = token });
        }

        // GET /api/Farmer/profile — Only logged-in Farmers can access
        [HttpGet("profile")]
        [Authorize(Roles = "Farmer")]
        public async Task<IActionResult> GetProfile()
        {
            // Read the farmer's ID from their JWT token
            var farmerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _farmerService.GetProfileAsync(farmerId);
            return Ok(result);
        }

        // GET /api/Farmer/{id} — used by CreatedAtAction above
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetProfile(int id)
        {
            var result = await _farmerService.GetProfileAsync(id);
            return Ok(result);
        }

        // PUT /api/Farmer/profile — Update your own profile
        [HttpPut("profile")]
        [Authorize(Roles = "Farmer")]
        public async Task<IActionResult> UpdateProfile([FromBody] FarmerRegisterDto dto)
        {
            var farmerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _farmerService.UpdateProfileAsync(farmerId, dto);
            return Ok(result);
        }
    }
}