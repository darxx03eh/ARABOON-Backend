using Araboon.Data.Helpers;
using Araboon.Service.Interfaces;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using MimeKit;
using System.Text;

namespace Araboon.Service.Implementations
{
    public class EmailService : IEmailService
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IHostEnvironment env;
        private readonly EmailSettings emailSettings;

        public EmailService(IHttpContextAccessor httpContextAccessor, IHostEnvironment env,
                            EmailSettings emailSettings)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.env = env;
            this.emailSettings = emailSettings;
        }
        public async Task<string> SendAuthenticationsEmailAsync(string email, string linkOrCode, string subject, string name)
        {
            try
            {
                using (var client = new SmtpClient())
                {
                    var message = new MimeMessage();
                    message.From.Add(new MailboxAddress("Araboon", emailSettings.FromEmail));
                    message.To.Add(new MailboxAddress(name, email));
                    message.Subject = subject;
                    message.Body = new TextPart(MimeKit.Text.TextFormat.Html)
                    {
                        Text = await GetLocalizedEmailBodyAsync(linkOrCode, subject)
                    };
                    await client.ConnectAsync(emailSettings.SmtpServer, emailSettings.Port, emailSettings.UseSSL);
                    await client.AuthenticateAsync(emailSettings.FromEmail, emailSettings.Password);
                    await client.SendAsync(message);
                    return "Success";
                }
            }
            catch (Exception exp)
            {
                return "Failed";
            }
        }

        public async Task<string> SendNotificationsEmailsAsync(
            string name,
            string mangaName,
            int chapterNo,
            string chapterTitle,
            string lang,
            string link,
            string email
        )
        {
            try
            {
                using (var client = new SmtpClient())
                {
                    var message = new MimeMessage();
                    message.From.Add(new MailboxAddress("Araboon", emailSettings.FromEmail));
                    message.To.Add(new MailboxAddress(name, email));
                    message.Subject = "New Chapter Available";
                    var filePath = Path.Combine(env.ContentRootPath, "EmailTemplates", "ChapterNotificationEmail.en.html");
                    var text = await System.IO.File.ReadAllTextAsync(filePath, Encoding.UTF8);
                    var replacements = new Dictionary<string, string>
                    {
                        { "{LINK}", link },
                        { "{YEAR}", DateTime.UtcNow.Year.ToString() },
                        { "{USERNAME}", name },
                        { "{MANGA_NAME}", mangaName },
                        { "{CHAPTER_NUMBER}", chapterNo.ToString() },
                        { "{CHAPTER_TITLE}", chapterTitle },
                        { "{LANGUAGE}", lang },
                    };
                    foreach (var kv in replacements)
                        text = text.Replace(kv.Key, kv.Value);
                    message.Body = new TextPart(MimeKit.Text.TextFormat.Html)
                    {
                        Text = text
                    };
                    await client.ConnectAsync(emailSettings.SmtpServer, emailSettings.Port, emailSettings.UseSSL);
                    await client.AuthenticateAsync(emailSettings.FromEmail, emailSettings.Password);
                    await client.SendAsync(message);
                    return "Success";
                }
            }
            catch (Exception exp)
            {
                return "Failed";
            }
        }

        private async Task<string> GetLocalizedEmailBodyAsync(string linkOrCode, string subject)
        {
            var Request = httpContextAccessor.HttpContext.Request;
            var lang = Request.Headers["Accept-Language"].ToString();
            if (!string.IsNullOrWhiteSpace(lang) && lang.Contains(','))
                lang = lang.Split(',')[0];
            string fileName = "";
            switch (subject)
            {
                case "Verification Email":
                    fileName = lang switch
                    {
                        "ar" => "ConfirmationEmail.ar.html",
                        "en" => "ConfirmationEmail.en.html",
                        _ => "ConfirmationEmail.en.html"
                    };
                    break;
                case "Forget Password":
                    fileName = lang switch
                    {
                        "ar" => "ForgetPassword.ar.html",
                        "en" => "ForgetPassword.en.html",
                        _ => "ForgetPassword.en.html",
                    };
                    break;
                case "Change Your Email":
                    fileName = lang switch
                    {
                        "ar" => "ChangeEmail.ar.html", 
                        "en" => "ChangeEmail.en.html",
                        _ => "ChangeEmail.en.html"
                    };
                    break;
            }
            var filePath = Path.Combine(env.ContentRootPath, "EmailTemplates", fileName);
            var htmlContent = await System.IO.File.ReadAllTextAsync(filePath, Encoding.UTF8);
            htmlContent = htmlContent.Replace("{linkOrCode}", linkOrCode);
            htmlContent = htmlContent.Replace("{year}", DateTime.UtcNow.Year.ToString());
            return htmlContent;
        }
    }
}
