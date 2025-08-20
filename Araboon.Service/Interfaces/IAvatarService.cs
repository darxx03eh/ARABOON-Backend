namespace Araboon.Service.Interfaces
{
    public interface IAvatarService
    {
        public Task<Stream> DownloadImageAsStreamAsync(String avatarUrl);
    }
}
