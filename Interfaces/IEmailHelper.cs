namespace LuxoraStore.Interfaces
{
    public interface IEmailHelper
    {
        Task<bool> SendOtpEmailAsync(string toEmail, string otpCode);
    }
}
