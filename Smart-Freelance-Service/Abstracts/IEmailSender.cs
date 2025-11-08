using Smart_Freelance_Data.Results;

namespace Smart_Freelance_Service.Abstracts
{
    public interface IEmailSender
    {
        Task SendAsync(EmailMessage emailMessage);
        Task SendAsync(string to, string subject, string content);
    }
}
