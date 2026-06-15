namespace CropDeals.Services.Interfaces
{
    public interface INotificationService
    {
        Task SendCropNotificationAsync(string toEmail, string dealerName, string cropName, string cropType, string location, decimal quantity);
    }
}