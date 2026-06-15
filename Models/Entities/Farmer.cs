namespace CropDeals.Models.Entities
{

    public class Farmer
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string BankAccountNumber { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;

        public BankAccount? BankAccount { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        //since one farmer can own multiple crops
        public ICollection<Crop> Crops { get; set; } = new List<Crop>();
    }
}