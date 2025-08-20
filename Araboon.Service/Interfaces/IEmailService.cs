namespace Araboon.Service.Interfaces
{
    public interface IEmailService
    {
        public Task<String> SendAuthenticationsEmailAsync(String email, String linkOrCode, String subject, String name);
    }
}
