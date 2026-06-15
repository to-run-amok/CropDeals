namespace CropDeals.Models.Entities
{
    public class Review
    {
        public int Id { get; set; }
        public int Rating { get; set; }          
        public string Comment { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // The reviewer
        public int DealerId { get; set; }
        public Dealer Dealer { get; set; } = null!;

        // Farmer being reviewd
        public int FarmerId { get; set; }
        public Farmer Farmer { get; set; } = null!;

        //crop associated with dealer-farmer transaction
        public int CropId { get; set; }
        public Crop Crop { get; set; } = null!;
    }
}