using CropDeals.Models.DTOs;

namespace CropDeals.Services.Interfaces
{
    public interface IReportService
    {
        Task<IEnumerable<CropReportDto>> GetCropReportAsync(ReportFilterDto filter);
        Task<IEnumerable<PaymentReportDto>> GetPaymentReportAsync(DateTime? fromDate, DateTime? toDate);
        Task<SummaryReportDto> GetSummaryReportAsync();
    }
}