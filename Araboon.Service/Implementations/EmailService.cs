using Araboon.Data.Helpers;
using Araboon.Service.Interfaces;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MimeKit;
using MimeKit.Text;
using System.Text;

namespace Araboon.Service.Implementations
{
    public class EmailService : IEmailService
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IHostEnvironment env;
        private readonly EmailSettings emailSettings;
        private readonly ILogger<EmailService> logger;

        public EmailService(
            IHttpContextAccessor httpContextAccessor,
            IHostEnvironment env,
            EmailSettings emailSettings,
            ILogger<EmailService> logger)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.env = env;
            this.emailSettings = emailSettings;
            this.logger = logger;
        }

        // ───────────────────────────────────────────────────────────────
        public async Task<string> SendAuthenticationsEmailAsync(string email, string linkOrCode, string subject, string name)
        {
            logger.LogInformation("Sending authentication email - إرسال إيميل توثيق | Email: {Email}", email);

            try
            {
                using var client = new SmtpClient();
                var message = new MimeMessage();

                message.From.Add(new MailboxAddress("Araboon", emailSettings.FromEmail));
                message.To.Add(new MailboxAddress(name, email));
                message.Subject = subject;
                message.Body = new TextPart(TextFormat.Html)
                {
                    Text = await GetLocalizedEmailBodyAsync(linkOrCode, subject)
                };

                await client.ConnectAsync(emailSettings.SmtpServer, emailSettings.Port, emailSettings.UseSSL);
                await client.AuthenticateAsync(emailSettings.FromEmail, emailSettings.Password);
                await client.SendAsync(message);

                logger.LogInformation("Authentication email sent successfully - تم إرسال إيميل التوثيق بنجاح");
                return "Success";
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to send authentication email - فشل إرسال إيميل التوثيق");
                return "Failed";
            }
        }

        // ───────────────────────────────────────────────────────────────
        public async Task<string> SendNotificationsEmailsAsync(
            string name,
            string mangaName,
            int chapterNo,
            string chapterTitle,
            string lang,
            string link,
            string email)
        {
            logger.LogInformation("Sending notification email - إرسال إشعار عبر الإيميل | Email: {Email}", email);

            try
            {
                using var client = new SmtpClient();
                var message = new MimeMessage();

                message.From.Add(new MailboxAddress("Araboon", emailSettings.FromEmail));
                message.To.Add(new MailboxAddress(name, email));
                message.Subject = "New Chapter Available";

                var filePath = Path.Combine(env.ContentRootPath, "EmailTemplates", "ChapterNotificationEmail.en.html");
                var text = await File.ReadAllTextAsync(filePath, Encoding.UTF8);

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

                foreach (var item in replacements)
                    text = text.Replace(item.Key, item.Value);

                message.Body = new TextPart(TextFormat.Html) { Text = text };

                await client.ConnectAsync(emailSettings.SmtpServer, emailSettings.Port, emailSettings.UseSSL);
                await client.AuthenticateAsync(emailSettings.FromEmail, emailSettings.Password);
                await client.SendAsync(message);

                logger.LogInformation("Notification email sent successfully - تم إرسال الإشعار عبر الإيميل بنجاح");
                return "Success";
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to send notification email - فشل إرسال إيميل الإشعار");
                return "Failed";
            }
        }

        // ───────────────────────────────────────────────────────────────
        private async Task<string> GetLocalizedEmailBodyAsync(string linkOrCode, string subject)
        {
            logger.LogInformation("Loading localized email template - تحميل قالب الإيميل حسب اللغة | Subject: {Subject}", subject);

            var request = httpContextAccessor.HttpContext?.Request;
            var lang = request?.Headers["Accept-Language"].ToString();

            if (!string.IsNullOrWhiteSpace(lang) && lang.Contains(','))
                lang = lang.Split(',')[0];

            string fileName = subject switch
            {
                "Verification Email" => lang switch
                {
                    "ar" => "ConfirmationEmail.ar.html",
                    "en" => "ConfirmationEmail.en.html",
                    _ => "ConfirmationEmail.en.html"
                },

                "Forget Password" => lang switch
                {
                    "ar" => "ForgetPassword.ar.html",
                    "en" => "ForgetPassword.en.html",
                    _ => "ForgetPassword.en.html"
                },

                "Change Your Email" => lang switch
                {
                    "ar" => "ChangeEmail.ar.html",
                    "en" => "ChangeEmail.en.html",
                    _ => "ChangeEmail.en.html"
                },

                _ => "Default.en.html"
            };

            var filePath = Path.Combine(env.ContentRootPath, "EmailTemplates", fileName);

            try
            {
                var content = await File.ReadAllTextAsync(filePath, Encoding.UTF8);

                content = content.Replace("{linkOrCode}", linkOrCode)
                                 .Replace("{year}", DateTime.UtcNow.Year.ToString());

                logger.LogInformation("Localized email template loaded successfully - تم تحميل قالب الإيميل بنجاح");
                return content;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to load email template - فشل تحميل قالب الإيميل");
                return "Error loading email template.";
            }
        }
    }
}