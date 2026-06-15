using CropDeals.Models.DTOs;
using CropDeals.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CropDeals.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        // POST /api/Payment 
        [HttpPost]
        [Authorize(Roles = "Dealer")] //only Dealer can make payment for a purchased crop
        public async Task<IActionResult> MakePayment([FromBody] PaymentRequestDto dto)
        {
            var dealerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _paymentService.MakePaymentAsync(dealerId, dto);
            return Ok(result);
        }

        // GET /api/Payment/my-payments 
        [HttpGet("my-payments")]
        [Authorize(Roles = "Dealer")] //only said Dealer can see all payments they made
        public async Task<IActionResult> GetMyPayments()
        {
            var dealerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _paymentService.GetDealerPaymentsAsync(dealerId);
            return Ok(result);
        }

        // GET /api/Payment/received 
        [HttpGet("received")]
        [Authorize(Roles = "Farmer")] //only said Farmer can see all payments they received
        public async Task<IActionResult> GetReceivedPayments()
        {
            var farmerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _paymentService.GetFarmerPaymentsAsync(farmerId);
            return Ok(result);
        }
    }
}