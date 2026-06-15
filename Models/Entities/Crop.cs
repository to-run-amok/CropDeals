namespace CropDeals.Models.Entities

{
    public class Crop
{
    public int Id { get; set; }
    public string Type { get; set; } = string.Empty;        
    public string Name { get; set; } = string.Empty;      
    public decimal Quantity { get; set; }                  
    public string Location { get; set; } = string.Empty;
    public string Status { get; set; } = "Available";    
    public DateTime PostedAt { get; set; } = DateTime.UtcNow;

    
    public int FarmerId { get; set; }
    public Farmer Farmer { get; set; } = null!;

    public decimal? AgreedPrice { get; set; }      
    public int? PurchasedByDealerId { get; set; }  
    public Dealer? PurchasedByDealer { get; set; }
    }
}