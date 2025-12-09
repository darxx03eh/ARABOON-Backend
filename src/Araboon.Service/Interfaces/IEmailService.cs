namespace Araboon.Service.Interfaces
{
    public interface IEmailService
    {
        public Task<string> SendAuthenticationsEmailAsync(string email, string linkOrCode, string subject, string name);
        public Task<string> SendNotificationsEmailsAsync(
            string name, 
            string mangaName, 
            int chapterNo, 
            string chapterTitle, 
            string lang, 
            string link,
            string email
        );
    }
}
