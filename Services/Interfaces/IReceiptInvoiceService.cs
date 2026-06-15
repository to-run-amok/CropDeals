using CropDeals.Models.DTOs;

namespace CropDeals.Services.Interfaces
{
    public interface IReceiptInvoiceService
    {
        // Dealer purchases a crop which generates a invoice, and receipt on farmers end
        Task<InvoiceResponseDto> PurchaseCropAsync(int dealerId, PurchaseCropDto dto);

        Task<IEnumerable<ReceiptResponseDto>> GetFarmerReceiptsAsync(int farmerId);

        Task<IEnumerable<InvoiceResponseDto>> GetDealerInvoicesAsync(int dealerId);
    }
}