using Araboon.Service.Interfaces;

namespace Araboon.Service.Implementations
{
    public class AvatarService : IAvatarService
    {
        public async Task<Stream> DownloadImageAsStreamAsync(String avatarUrl)
        {
            using var httpClient = new HttpClient();
            var imageBytes = await httpClient.GetByteArrayAsync(avatarUrl);
            var stream = new MemoryStream(imageBytes);
            return stream;
        }
    }
}
