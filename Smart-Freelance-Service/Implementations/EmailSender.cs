using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;
using Smart_Freelance_Data.Helpers;
using Smart_Freelance_Data.Results;
using Smart_Freelance_Service.Abstracts;
//using System.Net.Mail;

namespace Smart_Freelance_Service.Implementations
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailSettings _emailSettings;

        public EmailSender(EmailSettings emailSettings)
        {
            _emailSettings = emailSettings;
        }
        public async Task SendAsync(EmailMessage emailMessage)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_emailSettings.FromName, _emailSettings.FromAddress));
            message.To.Add(MailboxAddress.Parse(emailMessage.To));
            message.Subject = emailMessage.Subject;

            message.Body = new TextPart(emailMessage.IsHtml ? TextFormat.Html : TextFormat.Plain)
            {
                Text = emailMessage.Content
            };

            using var smtpClient = new SmtpClient();
            await smtpClient.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.Port, SecureSocketOptions.SslOnConnect);
            await smtpClient.AuthenticateAsync(_emailSettings.FromAddress, _emailSettings.Password);
            await smtpClient.SendAsync(message);
        }

        public async Task SendAsync(string to, string subject, string content)
        {
            await SendAsync(new EmailMessage
            {
                To = to,
                Subject = subject,
                Content = content
            });
        }
    }
}
