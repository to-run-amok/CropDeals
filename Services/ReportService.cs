using CropDeals.Data;
using CropDeals.Models.DTOs;
using CropDeals.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CropDeals.Services
{
    public class ReportService : IReportService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ReportService> _logger;

        public ReportService(AppDbContext context, ILogger<ReportService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<CropReportDto>> GetCropReportAsync(ReportFilterDto filter)
        {
            _logger.LogInformation("Generating crop report with filters");

            //get all the crops first
            var query = _context.Crops
                .Include(c => c.Farmer)
                .Include(c => c.PurchasedByDealer)
                .AsQueryable();

            // Apply filters one by one
            if (!string.IsNullOrEmpty(filter.CropType))
                query = query.Where(c => c.Type.ToLower() == filter.CropType.ToLower());

            if (!string.IsNullOrEmpty(filter.Status))
                query = query.Where(c => c.Status.ToLower() == filter.Status.ToLower());

            if (!string.IsNullOrEmpty(filter.Location))
                query = query.Where(c => c.Location.ToLower().Contains(filter.Location.ToLower()));

            if (filter.FromDate.HasValue)
                query = query.Where(c => c.PostedAt >= filter.FromDate.Value);

            if (filter.ToDate.HasValue)
                query = query.Where(c => c.PostedAt <= filter.ToDate.Value);

            var crops = await query.ToListAsync();

            _logger.LogInformation("Crop report generated with {Count} records", crops.Count);

            return crops.Select(c => new CropReportDto
            {
                CropId = c.Id,
                CropName = c.Name,
                CropType = c.Type,
                Quantity = c.Quantity,
                Location = c.Location,
                Status = c.Status,
                PostedAt = c.PostedAt,
                FarmerName = c.Farmer.Name,
                DealerName = c.PurchasedByDealer?.Name,
                AgreedPrice = c.AgreedPrice
            });
        }

        public async Task<IEnumerable<PaymentReportDto>> GetPaymentReportAsync(
            DateTime? fromDate, DateTime? toDate)
        {
            _logger.LogInformation("Generating payment report");

            var query = _context.Payments
                .Include(p => p.Farmer)
                .Include(p => p.Dealer)
                .Include(p => p.Crop)
                .AsQueryable();

            if (fromDate.HasValue)
                query = query.Where(p => p.PaidAt >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(p => p.PaidAt <= toDate.Value);

            var payments = await query
                .OrderByDescending(p => p.PaidAt)
                .ToListAsync();

            _logger.LogInformation("Payment report generated with {Count} records", payments.Count);

            return payments.Select(p => new PaymentReportDto
            {
                PaymentId = p.Id,
                TransactionId = p.TransactionId,
                Amount = p.Amount,
                PaymentMethod = p.PaymentMethod,
                Status = p.Status,
                PaidAt = p.PaidAt,
                FarmerName = p.Farmer.Name,
                DealerName = p.Dealer.Name,
                CropName = p.Crop.Name
            });
        }

        public async Task<SummaryReportDto> GetSummaryReportAsync()
        {
            _logger.LogInformation("Generating summary report");

            var totalFarmers = await _context.Farmers.CountAsync();
            var totalDealers = await _context.Dealers.CountAsync();
            var totalCrops = await _context.Crops.CountAsync();
            var totalSold = await _context.Crops.CountAsync(c => c.Status == "Sold");
            var totalAvailable = await _context.Crops.CountAsync(c => c.Status == "Available");
            var totalPayments = await _context.Payments.CountAsync();
            var totalRevenue = await _context.Payments.SumAsync(p => p.Amount);
            var totalReviews = await _context.Reviews.CountAsync();
            var avgRating = totalReviews > 0
                ? await _context.Reviews.AverageAsync(r => (double)r.Rating)
                : 0;

            return new SummaryReportDto
            {
                TotalFarmers = totalFarmers,
                TotalDealers = totalDealers,
                TotalCropsPosted = totalCrops,
                TotalCropsSold = totalSold,
                TotalCropsAvailable = totalAvailable,
                TotalPayments = totalPayments,
                TotalRevenueGenerated = totalRevenue,
                TotalReviews = totalReviews,
                AverageRating = Math.Round(avgRating, 2)
            };
        }
    }
}