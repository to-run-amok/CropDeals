using CropDeals.Models.DTOs;
using CropDeals.Models.Entities;
using CropDeals.Repositories.Interfaces;
using CropDeals.Services.Interfaces;

namespace CropDeals.Services
{
    public class ReceiptInvoiceService : IReceiptInvoiceService
    {
        private readonly IReceiptRepository _receiptRepo;
        private readonly IInvoiceRepository _invoiceRepo;
        private readonly ICropRepository _cropRepo;
        private readonly IDealerRepository _dealerRepo;
        private readonly IFarmerRepository _farmerRepo;
        private readonly ILogger<ReceiptInvoiceService> _logger;

        public ReceiptInvoiceService(
            IReceiptRepository receiptRepo,
            IInvoiceRepository invoiceRepo,
            ICropRepository cropRepo,
            IDealerRepository dealerRepo,
            IFarmerRepository farmerRepo,
            ILogger<ReceiptInvoiceService> logger)
        {
            _receiptRepo = receiptRepo;
            _invoiceRepo = invoiceRepo;
            _cropRepo = cropRepo;
            _dealerRepo = dealerRepo;
            _farmerRepo = farmerRepo;
            _logger = logger;
        }

        public async Task<InvoiceResponseDto> PurchaseCropAsync(int dealerId, PurchaseCropDto dto)
        {
            _logger.LogInformation("Dealer ID: {DealerId} purchasing crop ID: {CropId}", dealerId, dto.CropId);

           
            var crop = await _cropRepo.GetByIdAsync(dto.CropId)
                ?? throw new KeyNotFoundException($"Crop with ID {dto.CropId} not found.");

            
            if (crop.Status != "Available")
                throw new InvalidOperationException("This crop has already been sold.");

            
            var dealer = await _dealerRepo.GetByIdAsync(dealerId)
                ?? throw new KeyNotFoundException("Dealer not found.");

            var farmer = await _farmerRepo.GetByIdAsync(crop.FarmerId)
                ?? throw new KeyNotFoundException("Farmer not found.");

           
            crop.Status = "Sold";
            crop.AgreedPrice = dto.AgreedPrice;
            crop.PurchasedByDealerId = dealerId;
            await _cropRepo.UpdateAsync(crop);

            // Generate dummy Receipt 
            var receipt = new Receipt
            {
                CropName = crop.Name,
                Quantity = crop.Quantity,
                Amount = dto.AgreedPrice,
                DealerName = dealer.Name,
                FarmerId = crop.FarmerId,
                CropId = crop.Id
            };
            await _receiptRepo.CreateAsync(receipt);
            _logger.LogInformation("Receipt generated for farmer ID: {FarmerId}", crop.FarmerId);

            // Generate Dummy invoice
            var invoice = new Invoice
            {
                CropName = crop.Name,
                Quantity = crop.Quantity,
                Amount = dto.AgreedPrice,
                FarmerName = farmer.Name,
                DealerId = dealerId,
                CropId = crop.Id
            };
            var createdInvoice = await _invoiceRepo.CreateAsync(invoice);
            _logger.LogInformation("Invoice generated for dealer ID: {DealerId}", dealerId);

            return new InvoiceResponseDto
            {
                Id = createdInvoice.Id,
                CropName = createdInvoice.CropName,
                Quantity = createdInvoice.Quantity,
                Amount = createdInvoice.Amount,
                FarmerName = createdInvoice.FarmerName,
                GeneratedAt = createdInvoice.GeneratedAt
            };
        }

        public async Task<IEnumerable<ReceiptResponseDto>> GetFarmerReceiptsAsync(int farmerId)
        {
            _logger.LogInformation("Fetching receipts for farmer ID: {FarmerId}", farmerId);
            var receipts = await _receiptRepo.GetByFarmerIdAsync(farmerId);
            return receipts.Select(r => new ReceiptResponseDto
            {
                Id = r.Id,
                CropName = r.CropName,
                Quantity = r.Quantity,
                Amount = r.Amount,
                DealerName = r.DealerName,
                GeneratedAt = r.GeneratedAt
            });
        }

        public async Task<IEnumerable<InvoiceResponseDto>> GetDealerInvoicesAsync(int dealerId)
        {
            _logger.LogInformation("Fetching invoices for dealer ID: {DealerId}", dealerId);
            var invoices = await _invoiceRepo.GetByDealerIdAsync(dealerId);
            return invoices.Select(i => new InvoiceResponseDto
            {
                Id = i.Id,
                CropName = i.CropName,
                Quantity = i.Quantity,
                Amount = i.Amount,
                FarmerName = i.FarmerName,
                GeneratedAt = i.GeneratedAt
            });
        }
    }
}