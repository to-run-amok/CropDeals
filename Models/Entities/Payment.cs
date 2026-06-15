namespace CropDeals.Models.Entities
{
    public class Payment
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;  
        public string CardLastFourDigits { get; set; } = string.Empty;  
        public string Status { get; set; } = "Success"; 
        public DateTime PaidAt { get; set; } = DateTime.UtcNow;
        public string TransactionId { get; set; } = string.Empty;  

      
        public int DealerId { get; set; }
        public Dealer Dealer { get; set; } = null!;

        
        public int FarmerId { get; set; }
        public Farmer Farmer { get; set; } = null!;

       
        public int CropId { get; set; }
        public Crop Crop { get; set; } = null!;
    }
}