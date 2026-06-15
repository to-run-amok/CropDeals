namespace CropDeals.Models.DTOs
{
    // content dealer must send to make payment
    public class PaymentRequestDto
    {
        public int CropId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;   
        public string CardLastFourDigits { get; set; } = string.Empty;  
    }

    // what we must send back to client after payment
    public class PaymentResponseDto
    {
        public int Id { get; set; }
        public string TransactionId { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public string CardLastFourDigits { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime PaidAt { get; set; }
        public string FarmerName { get; set; } = string.Empty;
        public string CropName { get; set; } = string.Empty;
    }
}