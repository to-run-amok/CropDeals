namespace CropDeals.Models.Entities
{
    public class Subscription
    {
        public int Id { get; set; }
        public string CropType { get; set; } = string.Empty;  
        public DateTime SubscribedAt { get; set; } = DateTime.UtcNow;

        public int DealerId { get; set; }
        public Dealer Dealer { get; set; } = null!;
    }
}