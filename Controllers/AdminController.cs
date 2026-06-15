using ClosedXML.Excel;
using CropDeals.Models.DTOs;
using CropDeals.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CropDeals.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;
        private readonly IReportService _reportService;
        private readonly ILogger<AdminController> _logger;

        // 3 DIs via a single Constructor
        public AdminController(
            IAdminService adminService,
            IReportService reportService,
            ILogger<AdminController> logger)
        {
            _adminService = adminService;
            _reportService = reportService;
            _logger = logger;
        }

        // POST /api/Admin/login
        [HttpPost("login")]
        [AllowAnonymous]        //no token needed to access this
        public async Task<IActionResult> Login([FromBody] AdminLoginDto dto)
        {
            var token = await _adminService.LoginAsync(dto);
            return Ok(new { Token = token });
        }

        //GET /api/Admin/farmers
        [HttpGet("farmers")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllFarmers()
        {
            var farmers = await _adminService.GetAllFarmersAsync();
            return Ok(farmers);
        }

        //GET /api/Admin/dealers
        [HttpGet("dealers")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllDealers()
        {
            var dealers = await _adminService.GetAllDealersAsync();
            return Ok(dealers);
        }

        //PUT /api/Admin/farmers/{id}/toggle-status
        [HttpPut("farmers/{id}/toggle-status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ToggleFarmerStatus(int id)
        {
            await _adminService.ToggleFarmerStatusAsync(id);
            return Ok(new { Message = $"Farmer {id} status toggled successfully." });
        }

        // PUT /api/Admin/dealers/{id}/toggle-status
        [HttpPut("dealers/{id}/toggle-status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ToggleDealerStatus(int id)
        {
            await _adminService.ToggleDealerStatusAsync(id);
            return Ok(new { Message = $"Dealer {id} status toggled successfully." });
        }

        // PUT /api/Admin/farmers/{id}
        [HttpPut("farmers/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditFarmer(int id, [FromBody] FarmerRegisterDto dto)
        {
            var result = await _adminService.EditFarmerAsync(id, dto);
            return Ok(result);
        }

        // PUT /api/Admin/dealers/{id}
        [HttpPut("dealers/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditDealer(int id, [FromBody] DealerRegisterDto dto)
        {
            var result = await _adminService.EditDealerAsync(id, dto);
            return Ok(result);
        }

        // POST /api/Admin/dealers
        [HttpPost("dealers")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddDealer([FromBody] DealerRegisterDto dto)
        {
            var result = await _adminService.AddDealerAsync(dto);
            return Ok(result);
        }

        // GET /api/Admin/dealers/export
        [HttpGet("dealers/export")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ExportDealersToExcel()
        {
            _logger.LogInformation("Admin exporting dealers to Excel");

            var dealers = await _adminService.GetAllDealersAsync();

            //some closed XML bs

            using var workbook = new XLWorkbook();
            var sheet = workbook.Worksheets.Add("Dealers");

            sheet.Cell(1, 1).Value = "ID";
            sheet.Cell(1, 2).Value = "Name";
            sheet.Cell(1, 3).Value = "Email";
            sheet.Cell(1, 4).Value = "Phone";
            sheet.Cell(1, 5).Value = "Status";

            var headerRow = sheet.Row(1);
            headerRow.Style.Font.Bold = true;
            headerRow.Style.Fill.BackgroundColor = XLColor.FromHtml("#2E6DA4");
            headerRow.Style.Font.FontColor = XLColor.White;

            int row = 2;
            foreach (var dealer in dealers)
            {
                sheet.Cell(row, 1).Value = dealer.Id;
                sheet.Cell(row, 2).Value = dealer.Name;
                sheet.Cell(row, 3).Value = dealer.Email;
                sheet.Cell(row, 4).Value = dealer.Phone;
                sheet.Cell(row, 5).Value = dealer.IsActive ? "Active" : "Inactive";
                row++;
            }

            sheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;

            return File(
                stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "Dealers_Report.xlsx"
            );
        }

        // GET /api/Admin/reports/crops
        [HttpGet("reports/crops")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetCropReport(
            [FromQuery] string? cropType,
            [FromQuery] string? status,
            [FromQuery] string? location,
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate)
        {
            var filter = new ReportFilterDto
            {
                CropType = cropType,
                Status = status,
                Location = location,
                FromDate = fromDate,
                ToDate = toDate
            };

            var result = await _reportService.GetCropReportAsync(filter);
            return Ok(result);
        }

        // GET /api/Admin/reports/payments
        [HttpGet("reports/payments")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetPaymentReport(
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate)
        {
            var result = await _reportService.GetPaymentReportAsync(fromDate, toDate);
            return Ok(result);
        }

        // GET /api/Admin/reports/summary
        [HttpGet("reports/summary")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetSummaryReport()
        {
            var result = await _reportService.GetSummaryReportAsync();
            return Ok(result);
        }
    }
}