namespace Araboon.Service.Interfaces
{
    public interface ICloudinaryService
    {
        public Task<string> UploadDefaultAvatarAsync(Stream image, string folderName, string fileName);
    }
}
