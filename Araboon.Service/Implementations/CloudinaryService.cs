using Araboon.Service.Interfaces;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Araboon.Data.Helpers;

namespace Araboon.Service.Implementations
{
    public class CloudinaryService : ICloudinaryService
    {
        private readonly Cloudinary cloudinary;
        private readonly CloudinarySettings cloudinarySettings;

        public CloudinaryService(CloudinarySettings cloudinarySettings)
        {
            this.cloudinarySettings = cloudinarySettings;
            var account = new Account(cloudinarySettings.CloudName, cloudinarySettings.ApiKey, cloudinarySettings.ApiSecret);
            cloudinary = new Cloudinary(account);
        }

        public async Task<string> UploadDefaultAvatarAsync(Stream image, string folderName, string fileName)
        {
            using var stream = image;
            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription("image", stream),
                Folder = folderName,
                PublicId = Path.GetFileNameWithoutExtension(fileName),
                UseFilename = false,
                UniqueFilename = false,
                Overwrite = false
            };
            var result = await cloudinary.UploadAsync(uploadParams);
            return result.SecureUrl.ToString();
        }
    }
}
