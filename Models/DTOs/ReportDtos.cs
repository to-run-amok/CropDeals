namespace CropDeals.Models.DTOs
{
    public class ReportFilterDto
    {
        public string? CropType { get; set; }        
        public string? Status { get; set; }          
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? Location { get; set; }
    }

    public class CropReportDto
    {
        public int CropId { get; set; }
        public string CropName { get; set; } = string.Empty;
        public string CropType { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public string Location { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime PostedAt { get; set; }
        public string FarmerName { get; set; } = string.Empty;
        public string? DealerName { get; set; }
        public decimal? AgreedPrice { get; set; }
    }

    public class PaymentReportDto
    {
        public int PaymentId { get; set; }
        public string TransactionId { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime PaidAt { get; set; }
        public string FarmerName { get; set; } = string.Empty;
        public string DealerName { get; set; } = string.Empty;
        public string CropName { get; set; } = string.Empty;
    }

    public class SummaryReportDto
    {
        public int TotalFarmers { get; set; }
        public int TotalDealers { get; set; }
        public int TotalCropsPosted { get; set; }
        public int TotalCropsSold { get; set; }
        public int TotalCropsAvailable { get; set; }
        public decimal TotalRevenueGenerated { get; set; }
        public int TotalPayments { get; set; }
        public int TotalReviews { get; set; }
        public double AverageRating { get; set; }
    }
}