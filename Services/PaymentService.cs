using CropDeals.Models.DTOs;
using CropDeals.Models.Entities;
using CropDeals.Repositories.Interfaces;
using CropDeals.Services.Interfaces;

namespace CropDeals.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepo;
        private readonly ICropRepository _cropRepo;
        private readonly IFarmerRepository _farmerRepo;
        private readonly IDealerRepository _dealerRepo;
        private readonly ILogger<PaymentService> _logger;

        public PaymentService(
            IPaymentRepository paymentRepo,
            ICropRepository cropRepo,
            IFarmerRepository farmerRepo,
            IDealerRepository dealerRepo,
            ILogger<PaymentService> logger)
        {
            _paymentRepo = paymentRepo;
            _cropRepo = cropRepo;
            _farmerRepo = farmerRepo;
            _dealerRepo = dealerRepo;
            _logger = logger;
        }

        public async Task<PaymentResponseDto> MakePaymentAsync(int dealerId, PaymentRequestDto dto)
        {
            _logger.LogInformation("Dealer ID: {DealerId} initiating payment for crop ID: {CropId}", dealerId, dto.CropId);

            
            var crop = await _cropRepo.GetByIdAsync(dto.CropId)
                ?? throw new KeyNotFoundException($"Crop with ID {dto.CropId} not found.");

            
            if (crop.Status != "Sold")
                throw new InvalidOperationException("Payment can only be made for a purchased crop. Please purchase the crop first.");

           
            if (crop.PurchasedByDealerId != dealerId)
                throw new UnauthorizedAccessException("You can only pay for crops you have purchased.");

         
            var existingPayments = await _paymentRepo.GetByDealerIdAsync(dealerId);
            if (existingPayments.Any(p => p.CropId == dto.CropId))
                throw new InvalidOperationException("Payment has already been made for this crop.");

            var farmer = await _farmerRepo.GetByIdAsync(crop.FarmerId)
                ?? throw new KeyNotFoundException("Farmer not found.");

            // Generating a fake transaction ID
            var transactionId = $"TXN-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";

            // Dummy payment rec
            var payment = new Payment
            {
                Amount = dto.Amount,
                PaymentMethod = dto.PaymentMethod,
                CardLastFourDigits = dto.CardLastFourDigits,
                Status = "Success",  
                TransactionId = transactionId,
                DealerId = dealerId,
                FarmerId = crop.FarmerId,
                CropId = dto.CropId
            };

            var created = await _paymentRepo.CreateAsync(payment);
            _logger.LogInformation("Payment successful. Transaction ID: {TransactionId}", transactionId);

            return new PaymentResponseDto
            {
                Id = created.Id,
                TransactionId = created.TransactionId,
                Amount = created.Amount,
                PaymentMethod = created.PaymentMethod,
                CardLastFourDigits = created.CardLastFourDigits,
                Status = created.Status,
                PaidAt = created.PaidAt,
                FarmerName = farmer.Name,
                CropName = crop.Name
            };
        }

        public async Task<IEnumerable<PaymentResponseDto>> GetDealerPaymentsAsync(int dealerId)
        {
            _logger.LogInformation("Fetching payments for dealer ID: {DealerId}", dealerId);
            var payments = await _paymentRepo.GetByDealerIdAsync(dealerId);
            return payments.Select(p => new PaymentResponseDto
            {
                Id = p.Id,
                TransactionId = p.TransactionId,
                Amount = p.Amount,
                PaymentMethod = p.PaymentMethod,
                CardLastFourDigits = p.CardLastFourDigits,
                Status = p.Status,
                PaidAt = p.PaidAt,
                FarmerName = p.Farmer?.Name ?? "Unknown",   
                CropName = p.Crop?.Name ?? "Unknown"        
            });
        }

        public async Task<IEnumerable<PaymentResponseDto>> GetFarmerPaymentsAsync(int farmerId)
        {
            _logger.LogInformation("Fetching payments received by farmer ID: {FarmerId}", farmerId);

            var payments = await _paymentRepo.GetByFarmerIdAsync(farmerId);

            return payments.Select(p => new PaymentResponseDto
            {
                Id = p.Id,
                TransactionId = p.TransactionId,
                Amount = p.Amount,
                PaymentMethod = p.PaymentMethod,
                CardLastFourDigits = p.CardLastFourDigits,
                Status = p.Status,
                PaidAt = p.PaidAt,
                FarmerName = p.Farmer?.Name ?? "Unknown",   
                CropName = p.Crop?.Name ?? "Unknown"        
            });
        }
    }
}