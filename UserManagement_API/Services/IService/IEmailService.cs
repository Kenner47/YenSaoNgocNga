namespace UserManagement_API.Services.IService
{
    public interface IEmailService
    {
        Task SendVerifyEmailAsync(string toEmail, string subject, string body);

    }
}
