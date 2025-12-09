using Araboon.Service.Interfaces;
using Microsoft.Extensions.Logging;

namespace Araboon.Service.Implementations
{
    public class AvatarService : IAvatarService
    {
        private readonly ILogger<AvatarService> logger;

        public AvatarService(ILogger<AvatarService> logger)
        {
            this.logger = logger;
        }

        public async Task<Stream> DownloadImageAsStreamAsync(string avatarUrl)
        {
            logger.LogInformation(
                "Downloading avatar image - بدء تحميل صورة البروفايل | Url: {Url}",
                avatarUrl
            );
            try
            {
                using var httpClient = new HttpClient();
                logger.LogInformation(
                    "Sending HTTP request for avatar image - إرسال طلب HTTP للحصول على الصورة | Url: {Url}",
                    avatarUrl
                );
                var imageBytes = await httpClient.GetByteArrayAsync(avatarUrl);
                logger.LogInformation(
                    "Avatar image downloaded successfully - تم تنزيل صورة البروفايل بنجاح | Size: {Size} bytes",
                    imageBytes.Length
                );
                var stream = new MemoryStream(imageBytes);
                logger.LogInformation(
                    "Returning avatar image stream - إرجاع ستريم الصورة"
                );
                return stream;
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "Error downloading avatar image - خطأ أثناء تحميل صورة البروفايل | Url: {Url}",
                    avatarUrl
                );
                throw;
            }
        }
    }
}