namespace CropDeals.Models.Entities
{
    public class BankAccount
    {
        public int Id { get; set; }
        public string AccountNumber { get; set; } = string.Empty;
        public string BankName { get; set; } = string.Empty;
        public string IFSCCode { get; set; } = string.Empty;
        public string AccountHolderName { get; set; } = string.Empty;

        //Foreign Keys
        public int? FarmerId { get; set; }
        public int? DealerId { get; set; }

        //Navigation Property ( Joins applications ) 
        public Farmer? Farmer { get; set; }
        public Dealer? Dealer { get; set; }
    }
}