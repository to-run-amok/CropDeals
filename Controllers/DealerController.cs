using CropDeals.Models.DTOs;
using CropDeals.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CropDeals.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DealerController : ControllerBase
    {
        private readonly IDealerService _dealerService;

        public DealerController(IDealerService dealerService)
        {
            _dealerService = dealerService;
        }

        // POST /api/Dealer/register
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] DealerRegisterDto dto)
        {
            var result = await _dealerService.RegisterAsync(dto);
            return CreatedAtAction(nameof(GetProfile), new { id = result.Id }, result);
        }

        // POST /api/Dealer/login
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var token = await _dealerService.LoginAsync(dto);
            return Ok(new { Token = token });
        }

        // GET /api/Dealer/profile
        [HttpGet("profile")]
        [Authorize(Roles = "Dealer")]
        public async Task<IActionResult> GetProfile()
        {
            var dealerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _dealerService.GetProfileAsync(dealerId);
            return Ok(result);
        }

        // GET /api/Dealer/{id}
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetProfile(int id)
        {
            var result = await _dealerService.GetProfileAsync(id);
            return Ok(result);
        }

        // PUT /api/Dealer/profile
        [HttpPut("profile")]
        [Authorize(Roles = "Dealer")]
        public async Task<IActionResult> UpdateProfile([FromBody] DealerRegisterDto dto)
        {
            var dealerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _dealerService.UpdateProfileAsync(dealerId, dto);
            return Ok(result);
        }
    }
}
