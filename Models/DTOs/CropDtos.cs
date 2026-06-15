namespace CropDeals.Models.DTOs
{
    public class CropCreateDto
    {
        public string Type { get; set; } = string.Empty;      
        public string Name { get; set; } = string.Empty;     
        public decimal Quantity { get; set; }              
        public string Location { get; set; } = string.Empty;
    }

    public class CropResponseDto
    {
        public int Id { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public string Location { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime PostedAt { get; set; }
        public int FarmerId { get; set; }
        public string FarmerName { get; set; } = string.Empty;
    }
}