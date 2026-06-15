namespace CropDeals.Models.Entities
{
    public class Invoice
    {
        public int Id { get; set; }
        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
        public decimal Amount { get; set; }
        public decimal Quantity { get; set; }
        public string CropName { get; set; } = string.Empty;
        public string FarmerName { get; set; } = string.Empty;

        //only dealers will have Invoices
        public int DealerId { get; set; }
        public Dealer Dealer { get; set; } = null!;

 
        public int CropId { get; set; }
        public Crop Crop { get; set; } = null!;
    }
}