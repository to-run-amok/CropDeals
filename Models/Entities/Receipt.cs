namespace CropDeals.Models.Entities
{
    public class Receipt
    {
        public int Id { get; set; }
        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
        public decimal Amount { get; set; }
        public decimal Quantity { get; set; }
        public string CropName { get; set; } = string.Empty;
        public string DealerName { get; set; } = string.Empty;

        // only farmer will have recipts
        public int FarmerId { get; set; }
        public Farmer Farmer { get; set; } = null!;

       
        public int CropId { get; set; }
        public Crop Crop { get; set; } = null!;
    }
}