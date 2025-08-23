namespace Araboon.Service.Interfaces
{
    public interface IEmailService
    {
        public Task<string> SendAuthenticationsEmailAsync(string email, string linkOrCode, string subject, string name);
    }
}
