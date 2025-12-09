using Araboon.Data.Helpers;
using Araboon.Service.Interfaces;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Logging;

namespace Araboon.Service.Implementations
{
    public class CloudinaryService : ICloudinaryService
    {
        private readonly Cloudinary cloudinary;
        private readonly CloudinarySettings cloudinarySettings;
        private readonly ILogger<CloudinaryService> logger;

        public CloudinaryService(CloudinarySettings cloudinarySettings, ILogger<CloudinaryService> logger)
        {
            this.cloudinarySettings = cloudinarySettings;
            this.logger = logger;

            var account = new Account(cloudinarySettings.CloudName, cloudinarySettings.ApiKey, cloudinarySettings.ApiSecret);
            cloudinary = new Cloudinary(account);
        }

        public async Task<string> UploadFileAsync(Stream image, string folderName, string fileName)
        {
            logger.LogInformation("Uploading file to Cloudinary - رفع الملف إلى Cloudinary | Folder: {Folder}, FileName: {File}", folderName, fileName);

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

            logger.LogInformation("File uploaded successfully - تم رفع الملف بنجاح | Url: {Url}", result.SecureUrl.ToString());

            return result.SecureUrl.ToString();
        }

        public async Task<string> DeleteFileAsync(string url)
        {
            logger.LogInformation("Deleting file from Cloudinary - حذف الملف من Cloudinary | Url: {Url}", url);

            try
            {
                if (String.IsNullOrWhiteSpace(url))
                {
                    logger.LogWarning("Invalid URL provided - الرابط غير صالح");
                    return "InvalidPublicId";
                }

                var publicId = ExtractPublicIdFromUrl(url);

                logger.LogInformation("Extracted public ID - استخراج الـ PublicId | PublicId: {PublicId}", publicId);

                var deleteParams = new DeletionParams(publicId)
                {
                    Invalidate = true,
                    ResourceType = ResourceType.Image
                };

                var result = await cloudinary.DestroyAsync(deleteParams);

                if (result.Result.Equals("ok"))
                {
                    logger.LogInformation("Image deleted successfully - تم حذف الصورة بنجاح | PublicId: {PublicId}", publicId);
                    return "ImageDeletedSuccessfullyFromCloudinary";
                }

                logger.LogWarning("Failed to delete image - فشل حذف الصورة | PublicId: {PublicId}", publicId);
                return "FailedToDeleteImageFromCloudinary";
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error deleting image from Cloudinary - خطأ أثناء حذف الصورة من Cloudinary");
                return "AnErrorOccurredWhileDeletingFromCloudinary";
            }
        }

        private string ExtractPublicIdFromUrl(string url)
        {
            logger.LogInformation("Extracting PublicId from URL - استخراج PublicId من الرابط | Url: {Url}", url);

            var uri = new Uri(url);
            var segments = uri.AbsolutePath.Split('/').ToList();

            var startingIndex = segments.FindIndex(segment => segment.StartsWith("v")) + 1;
            var pathParts = segments.Skip(startingIndex);

            var publicId = String.Join("/", pathParts);

            var dotIndex = publicId.LastIndexOf(".");

            var finalId = dotIndex > 0 ? publicId.Substring(0, dotIndex) : publicId;

            logger.LogInformation("PublicId extracted - تم استخراج PublicId | PublicId: {Id}", finalId);

            return finalId;
        }
    }
}