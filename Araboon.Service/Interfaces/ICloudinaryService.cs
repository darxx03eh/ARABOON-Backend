namespace Araboon.Service.Interfaces
{
    public interface ICloudinaryService
    {
        public Task<string> UploadFileAsync(Stream image, string folderName, string fileName);
    }
}
