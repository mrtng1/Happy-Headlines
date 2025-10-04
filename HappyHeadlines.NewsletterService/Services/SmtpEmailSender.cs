using System.Net.Mail;

namespace HappyHeadlines.NewsletterService.Services
{
    public class SmtpEmailSender : IEmailSender
    {
        private readonly IConfiguration _cfg;
        public SmtpEmailSender(IConfiguration cfg) => _cfg = cfg;

        public async Task SendAsync(string to, string subject, string htmlBody, CancellationToken ct = default)
        {
            using var client = new SmtpClient(_cfg["Email:Smtp:Host"]!, int.Parse(_cfg["Email:Smtp:Port"]!));
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            var mail = new MailMessage(_cfg["Email:From"]!, to, subject, htmlBody) { IsBodyHtml = true };
            await client.SendMailAsync(mail, ct);
        }
    }
}
