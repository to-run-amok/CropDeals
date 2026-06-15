namespace CropDeals.Models.DTOs
{
    //from dealers end
    public class PurchaseCropDto
    {
        public int CropId { get; set; }
        public decimal AgreedPrice { get; set; }
    }

    public class ReceiptResponseDto
    {
        public int Id { get; set; }
        public string CropName { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public decimal Amount { get; set; }
        public string DealerName { get; set; } = string.Empty;
        public DateTime GeneratedAt { get; set; }
    }

    public class InvoiceResponseDto
    {
        public int Id { get; set; }
        public string CropName { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public decimal Amount { get; set; }
        public string FarmerName { get; set; } = string.Empty;
        public DateTime GeneratedAt { get; set; }
    }
}