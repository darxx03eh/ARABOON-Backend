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
        public async Task<string> UploadFileAsync(Stream image, string folderName, string fileName)
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
        public async Task<string> DeleteFileAsync(string url)
        {
            try
            {
                if (String.IsNullOrWhiteSpace(url))
                    return "InvalidPublicId";
                var publicId = ExtractPublicIdFromUrl(url);
                var deleteParams = new DeletionParams(publicId)
                {
                    Invalidate = true,
                    ResourceType = ResourceType.Image
                };
                var result = await cloudinary.DestroyAsync(deleteParams);
                return result.Result.Equals("ok") ? "ImageDeletedSuccessfullyFromCloudinary" : "FailedToDeleteImageFromCloudinary";

            }
            catch (Exception exp)
            {
                return "AnErrorOccurredWhileDeletingFromCloudinary";
            }
        }
        private string ExtractPublicIdFromUrl(string url)
        {
            var uri = new Uri(url);
            var segments = uri.AbsolutePath.Split('/').ToList();
            var startingIndex = segments.FindIndex(segment => segment.StartsWith("v")) + 1;
            var pathParts = segments.Skip(startingIndex);
            var publicId = String.Join("/", pathParts);
            var dotIndex = publicId.LastIndexOf(".");
            return dotIndex > 0 ? publicId.Substring(0, dotIndex) : publicId;
        }
    }
}
