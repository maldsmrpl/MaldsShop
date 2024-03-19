namespace MaldsShopWebApp.Interfaces
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync (string toEmail, string subject, string body);
    }
}
