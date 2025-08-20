namespace Araboon.Service.Interfaces
{
    public interface ICloudinaryService
    {
        public Task<String> UploadDefaultAvatarAsync(Stream image, String folderName, String fileName);
    }
}
