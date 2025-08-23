namespace Araboon.Service.Interfaces
{
    public interface IAvatarService
    {
        public Task<Stream> DownloadImageAsStreamAsync(string avatarUrl);
    }
}
