namespace CropDeals.Models.DTOs
{
    public class SubscriptionCreateDto
    {
        public string CropType { get; set; } = string.Empty;  
    }

    public class SubscriptionResponseDto
    {
        public int Id { get; set; }
        public string CropType { get; set; } = string.Empty;
        public DateTime SubscribedAt { get; set; }
    }
}