using CropDeals.Models.DTOs;
using CropDeals.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CropDeals.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BankAccountController : ControllerBase
    {
        private readonly IBankAccountService _bankService;

        public BankAccountController(IBankAccountService bankService)
        {
            _bankService = bankService;
        }

        // POST /api/BankAccount/farmer
        [HttpPost("farmer")]
        [Authorize(Roles = "Farmer")]
        public async Task<IActionResult> AddFarmerBankAccount([FromBody] BankAccountDto dto)
        {
            var farmerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _bankService.AddOrUpdateFarmerBankAccountAsync(farmerId, dto);
            return Ok(result);
        }

        // GET /api/BankAccount/farmer
        [HttpGet("farmer")]
        [Authorize(Roles = "Farmer")]
        public async Task<IActionResult> GetFarmerBankAccount()
        {
            var farmerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _bankService.GetFarmerBankAccountAsync(farmerId);
            return Ok(result);
        }

        // POST /api/BankAccount/dealer
        [HttpPost("dealer")]
        [Authorize(Roles = "Dealer")]
        public async Task<IActionResult> AddDealerBankAccount([FromBody] BankAccountDto dto)
        {
            var dealerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _bankService.AddOrUpdateDealerBankAccountAsync(dealerId, dto);
            return Ok(result);
        }

        // GET /api/BankAccount/dealer
        [HttpGet("dealer")]
        [Authorize(Roles = "Dealer")]
        public async Task<IActionResult> GetDealerBankAccount()
        {
            var dealerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _bankService.GetDealerBankAccountAsync(dealerId);
            return Ok(result);
        }
    }
}