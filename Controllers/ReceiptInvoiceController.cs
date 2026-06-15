using CropDeals.Models.DTOs;
using CropDeals.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CropDeals.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReceiptInvoiceController : ControllerBase
    {
        private readonly IReceiptInvoiceService _service;

        public ReceiptInvoiceController(IReceiptInvoiceService service)
        {
            _service = service;
        }

        // POST /api/ReceiptInvoice/purchase
        [HttpPost("purchase")]
        [Authorize(Roles = "Dealer")]
        public async Task<IActionResult> PurchaseCrop([FromBody] PurchaseCropDto dto)
        {
            var dealerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var invoice = await _service.PurchaseCropAsync(dealerId, dto);
            return Ok(invoice);
        }

        // GET /api/ReceiptInvoice/my-receipts
        [HttpGet("my-receipts")]
        [Authorize(Roles = "Farmer")] // only Farmer views their receipts
        public async Task<IActionResult> GetMyReceipts()
        {
            var farmerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var receipts = await _service.GetFarmerReceiptsAsync(farmerId);
            return Ok(receipts);
        }

        // GET /api/ReceiptInvoice/my-invoices
        [HttpGet("my-invoices")]
        [Authorize(Roles = "Dealer")] //only Dealer can view their invoices
        public async Task<IActionResult> GetMyInvoices()
        {
            var dealerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var invoices = await _service.GetDealerInvoicesAsync(dealerId);
            return Ok(invoices);
        }
    }
}