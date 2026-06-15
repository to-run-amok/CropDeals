using CropDeals.Services.Interfaces;
using System.Net;
using System.Net.Mail;

namespace CropDeals.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(IConfiguration config, ILogger<NotificationService> logger)
        {
            _config = config;
            _logger = logger;
        }

        public async Task SendCropNotificationAsync(
            string toEmail,
            string dealerName,
            string cropName,
            string cropType,
            string location,
            decimal quantity)
        {
            try
            {
                _logger.LogInformation("Sending crop notification to: {Email}", toEmail);

                var host = _config["Smtp:Host"]!;
                var port = int.Parse(_config["Smtp:Port"]!);
                var username = _config["Smtp:Username"]!;
                var password = _config["Smtp:Password"]!;
                var fromEmail = _config["Smtp:FromEmail"]!;
                var fromName = _config["Smtp:FromName"]!;

                var smtpClient = new SmtpClient(host)
                {
                    Port = port,
                    Credentials = new NetworkCredential(username, password),
                    EnableSsl = true
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(fromEmail, fromName),
                    Subject = $"New {cropType} Available — {cropName}!",
                    IsBodyHtml = true,
                    Body = $@"
                        <html>
                        <body style='font-family: Arial, sans-serif; padding: 20px;'>
                            <h2 style='color: #2E6DA4;'>New Crop Available on CropDeals!</h2>
                            <p>Hi <strong>{dealerName}</strong>,</p>
                            <p>A new crop matching your subscription has just been posted!</p>
                            <table style='border-collapse: collapse; width: 100%; margin: 20px 0;'>
                                <tr style='background-color: #2E6DA4; color: white;'>
                                    <th style='padding: 10px; text-align: left;'>Detail</th>
                                    <th style='padding: 10px; text-align: left;'>Info</th>
                                </tr>
                                <tr style='background-color: #EBF3FB;'>
                                    <td style='padding: 10px;'>Crop Name</td>
                                    <td style='padding: 10px;'><strong>{cropName}</strong></td>
                                </tr>
                                <tr>
                                    <td style='padding: 10px;'>Type</td>
                                    <td style='padding: 10px;'>{cropType}</td>
                                </tr>
                                <tr style='background-color: #EBF3FB;'>
                                    <td style='padding: 10px;'>Quantity Available</td>
                                    <td style='padding: 10px;'>{quantity} KG</td>
                                </tr>
                                <tr>
                                    <td style='padding: 10px;'>Location</td>
                                    <td style='padding: 10px;'>{location}</td>
                                </tr>
                            </table>
                            <p>Login to CropDeals now to purchase before it sells out!</p>
                            <p style='color: #888; font-size: 12px;'>You are receiving this because you subscribed to {cropType} crop notifications.</p>
                        </body>
                        </html>"
                };

                mailMessage.To.Add(toEmail);
                await smtpClient.SendMailAsync(mailMessage);

                _logger.LogInformation("Notification sent successfully to: {Email}", toEmail);
            }
            catch (Exception ex)
            {
                
                _logger.LogError(ex, "Failed to send notification to: {Email}", toEmail);
            }
        }
    }
}