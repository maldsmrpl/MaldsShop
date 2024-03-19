using MailKit.Net.Smtp;
using MaldsShopWebApp.Helpers;
using MaldsShopWebApp.Interfaces;
using Microsoft.AspNetCore.Identity.UI.Services;
using MimeKit;

namespace MaldsShopWebApp.Services
{
    public class EmailSender : IEmailSender
    {
        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("MaldsShop", "mail@malds.dev"));
            message.To.Add(new MailboxAddress("", toEmail));
            message.Subject = subject;
            message.Body = new TextPart("plain") 
            { 
                Text = body 
            };

            using (var client = new SmtpClient())
            {
                try
                {
                    await client.ConnectAsync("malds.dev", 465, true);
                    await client.AuthenticateAsync("mail@malds.dev", "MyMail-1803");

                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                    Console.WriteLine("Email sent successfully!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to send email. Error: {ex.Message}");
                }
            }
        }
    }
}
