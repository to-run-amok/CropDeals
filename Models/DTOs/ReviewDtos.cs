namespace CropDeals.Models.DTOs
{
    public class ReviewCreateDto
    {
        public int FarmerId { get; set; }
        public int CropId { get; set; }
        public int Rating { get; set; }       
        public string Comment { get; set; } = string.Empty;
    }

    public class ReviewResponseDto
    {
        public int Id { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
        public string DealerName { get; set; } = string.Empty;
        public string FarmerName { get; set; } = string.Empty;
        public string CropName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}