using Araboon.Data.DTOs.Mangas;
using Araboon.Data.Entities;
using Araboon.Data.Entities.Identity;
using Araboon.Data.Enums;
using Araboon.Data.Response.Categories.Queries;
using Araboon.Data.Response.Mangas.Queries;
using Araboon.Data.Wrappers;
using Araboon.Infrastructure.Commons;
using Araboon.Infrastructure.Data;
using Araboon.Infrastructure.IRepositories;
using Araboon.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

namespace Araboon.Service.Implementations
{
    public class MangaService : IMangaService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ICloudinaryService cloudinaryService;
        private readonly AraboonDbContext context;
        private readonly UserManager<AraboonUser> userManager;
        private readonly ILogger<MangaService> logger;

        public MangaService(IUnitOfWork unitOfWork, ICloudinaryService cloudinaryService, AraboonDbContext context,
            UserManager<AraboonUser> userManager, ILogger<MangaService> logger)
        {
            this.unitOfWork = unitOfWork;
            this.cloudinaryService = cloudinaryService;
            this.context = context;
            this.userManager = userManager;
            this.logger = logger;
        }

        public async Task<(string, IList<HomePageResponse>?, IList<string>?)> GetCategoriesHomePageAsync()
        {
            logger.LogInformation("Getting categories for home page - جلب التصنيفات لواجهة الصفحة الرئيسية");

            bool flag = await unitOfWork.MangaRepository.IsAdmin();
            var (message, list, categories) = await unitOfWork.MangaRepository.GetCategoriesHomePageAsync(flag);

            logger.LogInformation("GetCategoriesHomePageAsync result - نتيجة جلب التصنيفات للواجهة | Message: {Message}", message);

            return message switch
            {
                "MangaNotFound" => ("MangaNotFound", null, null),
                "MangaFound" => ("MangaFound", list, categories),
                _ => ("MangaNotFound", null, null)
            };
        }

        public async Task<(string, IList<GetHottestMangasResponse>?)> GetHottestMangasAsync()
        {
            logger.LogInformation("Getting hottest mangas - جلب أكثر المانجات شهرة");

            bool flag = await unitOfWork.MangaRepository.IsAdmin();
            var (message, hottestMangas) = await unitOfWork.MangaRepository.GetHottestMangasAsync(flag);

            logger.LogInformation("GetHottestMangasAsync result - نتيجة جلب المانجات الأكثر شهرة | Message: {Message}", message);

            return message switch
            {
                "MangaNotFound" => ("MangaNotFound", null),
                "MangaFound" => ("MangaFound", hottestMangas),
                _ => ("MangaNotFound", null)
            };
        }

        public async Task<(string, PaginatedResult<GetPaginatedHottestMangaResponse>?)> GetPaginatedHottestMangaAsync(int pageNumber, int pageSize)
        {
            logger.LogInformation("Getting paginated hottest mangas - جلب المانجات الأكثر شهرة مع صفحات | Page: {Page}, Size: {Size}", pageNumber, pageSize);

            bool flag = await unitOfWork.MangaRepository.IsAdmin();
            var (message, hottestMangas) = await unitOfWork.MangaRepository.GetPaginatedHottestMangaAsync(pageNumber, pageSize, flag);

            logger.LogInformation("GetPaginatedHottestMangaAsync result - نتيجة جلب المانجات الأكثر شهرة | Message: {Message}", message);

            return message switch
            {
                "MangaNotFound" => ("MangaNotFound", null),
                "MangaFound" => ("MangaFound", hottestMangas),
                _ => ("MangaNotFound", null)
            };
        }

        public async Task<(string, PaginatedResult<GetMangaByCategoryNameResponse>?)> GetMangaByCategoryNameAsync(string category, int pageNumber, int pageSize)
        {
            logger.LogInformation("Getting mangas by category name - جلب المانجات حسب اسم التصنيف | Category: {Category}, Page: {Page}, Size: {Size}", category, pageNumber, pageSize);

            bool flag = await unitOfWork.MangaRepository.IsAdmin();
            var (message, mangas) = await unitOfWork.MangaRepository.GetMangaByCategoryNameAsync(category, pageNumber, pageSize, flag);

            logger.LogInformation("GetMangaByCategoryNameAsync result - نتيجة جلب المانجات حسب التصنيف | Message: {Message}", message);

            return message switch
            {
                "MangaNotFound" => ("MangaNotFound", null),
                "MangaFound" => ("MangaFound", mangas),
                _ => ("MangaNotFound", null)
            };
        }

        public async Task<(string, PaginatedResult<GetMangaByStatusResponse>?)> GetMangaByStatusAsync(int pageNumber, int pageSize, string status, MangaOrderingEnum orderBy, string? filter)
        {
            logger.LogInformation("Getting mangas by status - جلب المانجات حسب الحالة | Status: {Status}, OrderBy: {OrderBy}, Filter: {Filter}, Page: {Page}, Size: {Size}",
                status, orderBy, filter, pageNumber, pageSize);

            bool flag = await unitOfWork.MangaRepository.IsAdmin();
            var (message, mangas) = await unitOfWork.MangaRepository.GetMangaByStatusAsync(pageNumber, pageSize, status, orderBy, filter, flag);

            logger.LogInformation("GetMangaByStatusAsync result - نتيجة جلب المانجات حسب الحالة | Message: {Message}", message);

            return message switch
            {
                "MangaNotFound" => ("MangaNotFound", null),
                "MangaFound" => ("MangaFound", mangas),
                _ => ("MangaNotFound", null)
            };
        }

        public async Task<(string, Manga?, string?, int?)> GetMangaByIDAsync(int id)
        {
            logger.LogInformation("Getting manga by ID - جلب المانجا حسب المعرف | MangaId: {Id}", id);

            bool flag = await unitOfWork.MangaRepository.IsAdmin();
            var manga = await unitOfWork.MangaRepository.GetByIdAsync(id);
            if (manga is null)
            {
                logger.LogWarning("Manga not found - المانجا غير موجودة | MangaId: {Id}", id);
                return ("MangaNotFound", null, null, null);
            }

            if (flag ? false : !manga.IsActive)
            {
                logger.LogWarning("Manga is not active for non-admin - المانجا غير مفعلة للمستخدم العادي | MangaId: {Id}", id);
                return ("MangaNotFound", null, null, null);
            }

            var commentCounts = await unitOfWork.MangaRepository.CommentsCountByIdAsync(id);

            logger.LogInformation("Manga found - تم العثور على المانجا | MangaId: {Id}, CommentsCount: {Comments}", id, commentCounts);

            return ("MangaFound", manga, manga.MangaNameEn, commentCounts);
        }

        public async Task<(string, PaginatedResult<MangaSearchResponse>?)> SearchAsync(string? search, int pageNumber, int pageSize)
        {
            logger.LogInformation("Searching mangas - البحث عن مانجات | Query: {Search}, Page: {Page}, Size: {Size}", search, pageNumber, pageSize);

            bool flag = await unitOfWork.MangaRepository.IsAdmin();
            var (message, mangas) = await unitOfWork.MangaRepository.SearchAsync(search, pageNumber, pageSize, flag);

            logger.LogInformation("SearchAsync result - نتيجة البحث عن المانجات | Message: {Message}", message);

            return message switch
            {
                "MangaNotFound" => ("MangaNotFound", null),
                "MangaFound" => ("MangaFound", mangas),
                _ => ("MangaNotFound", null)
            };
        }

        public async Task<(string, PaginatedResult<GetMangaCommentsResponse>?)> GetMangaCommentsAsync(int id, int pageNumber, int pageSize)
        {
            logger.LogInformation("Getting manga comments - جلب تعليقات المانجا | MangaId: {Id}, Page: {Page}, Size: {Size}", id, pageNumber, pageSize);

            var (message, comments) = await unitOfWork.MangaRepository.GetMangaCommentsAsync(id, pageNumber, pageSize);

            logger.LogInformation("GetMangaCommentsAsync result - نتيجة جلب تعليقات المانجا | Message: {Message}", message);

            return message switch
            {
                "CommentsNotFound" => ("CommentsNotFound", null),
                "CommentsFound" => ("CommentsFound", comments),
                _ => ("CommentsNotFound", null)
            };
        }

        public async Task<(string, int?)> GetCommentsCountAsync(int id)
        {
            logger.LogInformation("Getting comments count for manga - جلب عدد التعليقات للمانجا | MangaId: {Id}", id);

            var (message, commentsCount) = await unitOfWork.CommentRepository.GetCommentsCountForMangaAsync(id);

            logger.LogInformation("GetCommentsCountAsync result - نتيجة جلب عدد التعليقات | Message: {Message}, Count: {Count}", message, commentsCount);

            return message switch
            {
                "MangaNotFound" => ("MangaNotFound", null),
                "CommentsNotFound" => ("CommentsNotFound", null),
                "CommentsFound" => ("CommentsFound", commentsCount),
                _ => ("MangaNotFound", null)
            };
        }

        public async Task<(string, GetMangaForDashboardResponse?)> AddNewMangaAsync(MangaInfoDTO mangaInfo)
        {
            logger.LogInformation("Adding new manga - إضافة مانجا جديدة | NameEn: {NameEn}, NameAr: {NameAr}", mangaInfo.MangaNameEn, mangaInfo.MangaNameAr);

            await using var transaction = await context.Database.BeginTransactionAsync();

            try
            {
                var categories = new List<Category>();
                if (mangaInfo.CategoriesIds is not null)
                {
                    foreach (var categoryId in mangaInfo.CategoriesIds)
                    {
                        var category = await unitOfWork.CategoryRepository.GetByIdAsync(categoryId);
                        if (category is null)
                        {
                            logger.LogWarning("Category not found while adding manga - لم يتم العثور على التصنيف أثناء إضافة المانجا | CategoryId: {CategoryId}", categoryId);
                            return ("CategoryNotFound", null);
                        }
                        categories.Add(category);
                    }
                }

                var manga = new Manga
                {
                    MangaNameEn = mangaInfo.MangaNameEn,
                    MangaNameAr = mangaInfo.MangaNameAr,
                    StatusEn = mangaInfo.StatusEn,
                    StatusAr = mangaInfo.StatusAr,
                    AuthorEn = mangaInfo.AuthorEn,
                    AuthorAr = mangaInfo.AuthorAr,
                    TypeEn = mangaInfo.TypeEn,
                    TypeAr = mangaInfo.TypeAr,
                    DescriptionEn = mangaInfo.DescriptionEn,
                    DescriptionAr = mangaInfo.DescriptionAr,
                    MainImage = null
                };

                var mangaResult = await unitOfWork.MangaRepository.AddAsync(manga);
                if (mangaResult is null)
                {
                    logger.LogError("Error adding manga - خطأ أثناء إضافة المانجا");
                    return ("ThereWasAProblemAddingTheManga", null);
                }

                mangaResult.CategoryMangas = categories
                    .Select(c => new CategoryManga { MangaID = mangaResult.MangaID, CategoryID = c.CategoryID })
                    .ToList();

                if (mangaInfo.Image is not null)
                {
                    logger.LogInformation("Uploading manga image - رفع صورة المانجا | MangaId: {MangaId}", mangaResult.MangaID);

                    var url = await UploadMangaImageAsync(mangaInfo.Image, mangaResult.MangaID);
                    if (url is null)
                    {
                        logger.LogError("Error uploading manga image - خطأ أثناء رفع صورة المانجا | MangaId: {MangaId}", mangaResult.MangaID);
                        return ("AnErrorOccurredWhileAddingTheImageForManga", null);
                    }
                    mangaResult.MainImage = url;
                }

                await unitOfWork.MangaRepository.UpdateAsync(mangaResult);
                await transaction.CommitAsync();

                logger.LogInformation("Manga added successfully - تم إضافة المانجا بنجاح | MangaId: {MangaId}", mangaResult.MangaID);

                return ("MangaAddedSuccessfully", new GetMangaForDashboardResponse()
                {
                    MangaID = mangaResult.MangaID,
                    MangaName = TransableEntity.GetTransable(mangaResult.MangaNameEn, mangaResult.MangaNameAr),
                    MangaImageUrl = mangaResult.MainImage,
                    AuthorName = TransableEntity.GetTransable(mangaResult.AuthorEn, mangaResult.AuthorAr),
                    IsFavorite = null,
                    LastChapter = mangaResult.Chapters.OrderByDescending(chapter => chapter.ChapterNo)
                        .Select(chapter => new LastChapter()
                        {
                            ChapterID = chapter.ChapterID,
                            ChapterNo = chapter.ChapterNo,
                            Views = chapter.ReadersCount
                        }).FirstOrDefault(),
                    Name = new MangaName() { En = mangaResult.MangaNameEn, Ar = mangaResult.MangaNameAr },
                    Description = new Description() { En = mangaResult.DescriptionEn, Ar = mangaResult.DescriptionAr },
                    Author = new Author() { En = mangaResult.AuthorEn, Ar = mangaResult.AuthorAr },
                    Type = new Araboon.Data.Response.Mangas.Queries.Type() { En = mangaResult.TypeEn, Ar = mangaResult.TypeAr },
                    Status = new Status() { En = mangaResult.StatusEn, Ar = mangaResult.StatusAr },
                    Categories = mangaResult.CategoryMangas.Select(category => new CategoriesResponse()
                    {
                        Id = category.CategoryID,
                        En = category.Category.CategoryNameEn,
                        Ar = category.Category.CategoryNameAr
                    }).ToList(),
                    IsActive = manga.IsActive,
                    IsArabicAvailable = Convert.ToBoolean(manga.ArabicAvailable),
                    IsEnglishAvailable = Convert.ToBoolean(manga.EnglishAvilable)
                });
            }
            catch (Exception exp)
            {
                logger.LogError(exp, "Error while adding manga - خطأ أثناء إضافة المانجا");

                if (transaction.GetDbTransaction().Connection is not null)
                    await transaction.RollbackAsync();

                return ("AnErrorOccurredWhileAddingTheManga", null);
            }
        }

        public async Task<(string, GetMangaForDashboardResponse?)> UpdateExistMangaAsync(UpdateMangaInfoDTO mangaInfo, int mangaId)
        {
            logger.LogInformation("Updating existing manga - تعديل مانجا موجودة | MangaId: {MangaId}", mangaId);

            var manga = await unitOfWork.MangaRepository.GetByIdAsync(mangaId);
            if (manga is null)
            {
                logger.LogWarning("Manga not found for update - المانجا غير موجودة للتعديل | MangaId: {MangaId}", mangaId);
                return ("MangaNotFound", null);
            }

            var categories = new List<Category>();
            foreach (var categoryId in mangaInfo.CategoriesIds)
            {
                var category = await unitOfWork.CategoryRepository.GetByIdAsync(categoryId);
                if (category is null)
                {
                    logger.LogWarning("Category not found while updating manga - لم يتم العثور على التصنيف أثناء التعديل | CategoryId: {CategoryId}", categoryId);
                    return ("CategoryNotFound", null);
                }
                categories.Add(category);
            }

            var deletedCategories = manga.CategoryMangas.ToList();
            await using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                await unitOfWork.CategoryMangaRepository.DeleteRangeAsync(deletedCategories);

                manga.MangaNameEn = mangaInfo.MangaNameEn;
                manga.MangaNameAr = mangaInfo.MangaNameAr;
                manga.StatusEn = mangaInfo.StatusEn;
                manga.StatusAr = mangaInfo.StatusAr;
                manga.AuthorEn = mangaInfo.AuthorEn;
                manga.AuthorAr = mangaInfo.AuthorAr;
                manga.TypeEn = mangaInfo.TypeEn;
                manga.TypeAr = mangaInfo.TypeAr;
                manga.DescriptionEn = mangaInfo.DescriptionEn;
                manga.DescriptionAr = mangaInfo.DescriptionAr;
                manga.CategoryMangas = categories
                    .Select(c => new CategoryManga { MangaID = mangaId, CategoryID = c.CategoryID })
                    .ToList();

                await unitOfWork.MangaRepository.UpdateAsync(manga);
                await transaction.CommitAsync();

                logger.LogInformation("Manga updated successfully - تم تعديل المانجا بنجاح | MangaId: {MangaId}", mangaId);

                return ("MangaUpdatingSuccessfully", new GetMangaForDashboardResponse()
                {
                    MangaID = manga.MangaID,
                    MangaName = TransableEntity.GetTransable(manga.MangaNameEn, manga.MangaNameAr),
                    MangaImageUrl = manga.MainImage,
                    AuthorName = TransableEntity.GetTransable(manga.AuthorEn, manga.AuthorAr),
                    IsFavorite = null,
                    LastChapter = manga.Chapters.OrderByDescending(chapter => chapter.ChapterNo)
                        .Select(chapter => new LastChapter()
                        {
                            ChapterID = chapter.ChapterID,
                            ChapterNo = chapter.ChapterNo,
                            Views = chapter.ReadersCount
                        }).FirstOrDefault(),
                    Name = new MangaName() { En = manga.MangaNameEn, Ar = manga.MangaNameAr },
                    Description = new Description() { En = manga.DescriptionEn, Ar = manga.DescriptionAr },
                    Author = new Author() { En = manga.AuthorEn, Ar = manga.AuthorAr },
                    Type = new Araboon.Data.Response.Mangas.Queries.Type() { En = manga.TypeEn, Ar = manga.TypeAr },
                    Status = new Status() { En = manga.StatusEn, Ar = manga.StatusAr },
                    Categories = manga.CategoryMangas.Select(category => new CategoriesResponse()
                    {
                        Id = category.CategoryID,
                        En = category.Category.CategoryNameEn,
                        Ar = category.Category.CategoryNameAr
                    }).ToList(),
                    IsActive = manga.IsActive,
                    IsArabicAvailable = Convert.ToBoolean(manga.ArabicAvailable),
                    IsEnglishAvailable = Convert.ToBoolean(manga.EnglishAvilable)
                });
            }
            catch (Exception exp)
            {
                logger.LogError(exp, "Error while updating manga - خطأ أثناء تعديل المانجا | MangaId: {MangaId}", mangaId);

                if (transaction.GetDbTransaction().Connection is not null)
                    await transaction.RollbackAsync();

                return ("AnErrorOccurredWhileUpdatingTheManga", null);
            }
        }

        private async Task<string?> UploadMangaImageAsync(IFormFile image, int mangaId)
        {
            logger.LogInformation("Uploading manga image (private) - رفع صورة المانجا (خاص) | MangaId: {MangaId}", mangaId);

            try
            {
                var guidPart = Guid.NewGuid().ToString("N").Substring(0, 12);
                var datePart = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
                var id = $"{guidPart}-{datePart}";

                using var stream = image.OpenReadStream();
                var folderName = $"ARABOON/Mangas/{mangaId}/img";
                var url = await cloudinaryService.UploadFileAsync(stream, folderName, id);

                logger.LogInformation("Manga image uploaded (private) - تم رفع صورة المانجا (خاص) | MangaId: {MangaId}", mangaId);

                return url;
            }
            catch (Exception exp)
            {
                logger.LogError(exp, "Error uploading manga image (private) - خطأ أثناء رفع صورة المانجا (خاص) | MangaId: {MangaId}", mangaId);
                return null;
            }
        }

        public async Task<string> DeleteMangaAsync(int id)
        {
            logger.LogInformation("Deleting manga - حذف المانجا | MangaId: {MangaId}", id);

            var manga = await unitOfWork.MangaRepository.GetByIdAsync(id);
            if (manga is null)
            {
                logger.LogWarning("Manga not found for delete - المانجا غير موجودة للحذف | MangaId: {MangaId}", id);
                return "MangaNotFound";
            }

            var comments = await unitOfWork.CommentRepository.GetTableNoTracking()
                           .Where(comment => comment.MangaID.Equals(id)).ToListAsync();

            var chapters = await unitOfWork.ChapterRepository.GetTableNoTracking()
                           .Where(chapter => chapter.MangaID.Equals(id)).ToListAsync();

            await using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                if (comments is not null)
                    await unitOfWork.CommentRepository.DeleteRangeAsync(comments);
                if (chapters is not null)
                    await unitOfWork.ChapterRepository.DeleteRangeAsync(chapters);

                var categories = manga.CategoryMangas.ToList();
                if (categories.Any())
                {
                    foreach (var category in categories)
                    {
                        var mangaInCategory = await unitOfWork.CategoryMangaRepository.GetTableNoTracking()
                                              .CountAsync(c => c.CategoryID.Equals(category.CategoryID));
                        if (mangaInCategory.Equals(1))
                            category.Category.IsActive = false;
                    }
                }

                await unitOfWork.MangaRepository.DeleteAsync(manga);
                await transaction.CommitAsync();

                logger.LogInformation("Manga deleted successfully - تم حذف المانجا بنجاح | MangaId: {MangaId}", id);

                return "MangaDeletedSuccessfully";
            }
            catch (Exception exp)
            {
                logger.LogError(exp, "Error deleting manga - خطأ أثناء حذف المانجا | MangaId: {MangaId}", id);

                if (transaction.GetDbTransaction().Connection is not null)
                    await transaction.RollbackAsync();

                return "AnErrorOccurredWhileDeletingTheManga";
            }
        }

        public async Task<string> DeleteMangaImageAsync(int id)
        {
            logger.LogInformation("Deleting manga image - حذف صورة المانجا | MangaId: {MangaId}", id);

            var manga = await unitOfWork.MangaRepository.GetByIdAsync(id);
            if (manga is null)
            {
                logger.LogWarning("Manga not found for deleting image - المانجا غير موجودة لحذف الصورة | MangaId: {MangaId}", id);
                return "MangaNotFound";
            }

            var url = manga.MainImage;
            if (string.IsNullOrWhiteSpace(url))
            {
                logger.LogWarning("No image to delete - لا توجد صورة لحذفها | MangaId: {MangaId}", id);
                return "ThereIsNoImageToDelete";
            }

            try
            {
                var cloudinaryResult = cloudinaryService.DeleteFileAsync(url);
                if (cloudinaryResult.Equals("FailedToDeleteImageFromCloudinary"))
                {
                    logger.LogError("Failed to delete image from Cloudinary - فشل حذف الصورة من Cloudinary | MangaId: {MangaId}", id);
                    return "FailedToDeleteImageFromCloudinary";
                }

                manga.MainImage = null;
                await unitOfWork.MangaRepository.UpdateAsync(manga);

                logger.LogInformation("Manga image deleted successfully - تم حذف صورة المانجا بنجاح | MangaId: {MangaId}", id);

                return "ImageHasBeenSuccessfullyDeleted";
            }
            catch (Exception exp)
            {
                logger.LogError(exp, "Error deleting manga image - خطأ أثناء حذف صورة المانجا | MangaId: {MangaId}", id);
                return "AnErrorOccurredWhileDeletingTheImage";
            }
        }

        public async Task<(string, string?)> UploadMangaImageAsync(int id, IFormFile image)
        {
            logger.LogInformation("Uploading manga image (public) - رفع صورة المانجا (عام) | MangaId: {MangaId}", id);

            var manga = await unitOfWork.MangaRepository.GetByIdAsync(id);
            if (manga is null)
            {
                logger.LogWarning("Manga not found for uploading image - المانجا غير موجودة لرفع الصورة | MangaId: {MangaId}", id);
                return ("MangaNotFound", null);
            }

            await using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                var originalUrl = manga.MainImage;
                if (!string.IsNullOrWhiteSpace(originalUrl))
                {
                    var cloudinaryResult = await cloudinaryService.DeleteFileAsync(originalUrl);
                    if (cloudinaryResult.Equals("FailedToDeleteImageFromCloudinary"))
                    {
                        logger.LogError("Failed to delete old image from Cloudinary - فشل حذف الصورة القديمة من Cloudinary | MangaId: {MangaId}", id);
                        return ("FailedToDeleteOldImageFromCloudinary", null);
                    }
                }

                var guidPart = Guid.NewGuid().ToString("N").Substring(0, 12);
                var datePart = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
                var fullPart = $"{guidPart}-{datePart}";

                using (var stream = image.OpenReadStream())
                {
                    var (imageName, folderName) = (fullPart, $"ARABOON/Mangas/{manga.MangaID}/img");
                    var url = await cloudinaryService.UploadFileAsync(stream, folderName, imageName);
                    manga.MainImage = url;
                }

                await unitOfWork.MangaRepository.UpdateAsync(manga);
                await transaction.CommitAsync();

                logger.LogInformation("Manga image changed successfully - تم تغيير صورة المانجا بنجاح | MangaId: {MangaId}", id);

                return ("TheImageHasBeenChangedSuccessfully", manga.MainImage);
            }
            catch (Exception exp)
            {
                logger.LogError(exp, "Error processing image modification request - خطأ أثناء معالجة طلب تعديل الصورة | MangaId: {MangaId}", id);

                if (transaction.GetDbTransaction().Connection is not null)
                    await transaction.RollbackAsync();

                return ("AnErrorOccurredWhileProcessingImageModificationRequest", null);
            }
        }

        public async Task<string> MakeArabicAvailableOrUnAvailableAsync(int id)
        {
            logger.LogInformation("Toggling Arabic availability - تبديل توفر اللغة العربية | MangaId: {MangaId}", id);

            var manga = await unitOfWork.MangaRepository.GetByIdAsync(id);
            if (manga is null)
            {
                logger.LogWarning("Manga not found for Arabic availability toggle - المانجا غير موجودة لتبديل العربية | MangaId: {MangaId}", id);
                return "MangaNotFound";
            }

            try
            {
                if (Convert.ToBoolean(manga.ArabicAvailable))
                {
                    manga.ArabicAvailable = false;
                }
                else
                {
                    var chaptersForLang = await context.Chapters
                        .Where(c => c.MangaID.Equals(manga.MangaID) && c.Language.ToLower().Equals("arabic"))
                        .OrderBy(c => c.ChapterNo)
                        .Select(c => c.ChapterNo)
                        .ToListAsync();

                    bool noGapsAndStartsAtOne = false;
                    if (chaptersForLang.Count > 0)
                    {
                        var ordered = chaptersForLang.OrderBy(n => n).ToList();
                        noGapsAndStartsAtOne = ordered.First() == 1 &&
                                               ordered.Zip(ordered.Skip(1), (a, b) => b - a).All(diff => diff == 1);
                    }
                    manga.ArabicAvailable = noGapsAndStartsAtOne;
                    if (!noGapsAndStartsAtOne)
                    {
                        logger.LogWarning("Cannot make Arabic available due to incomplete chapters - لا يمكن تفعيل العربية بسبب فصول غير مكتملة | MangaId: {MangaId}", id);
                        return "CanNotMakeArabicAvialableDueToIncompleteChapters";
                    }
                }

                await unitOfWork.MangaRepository.UpdateAsync(manga);

                if (Convert.ToBoolean(manga.ArabicAvailable))
                {
                    logger.LogInformation("Arabic made available for this manga - تم تفعيل العربية لهذه المانجا | MangaId: {MangaId}", id);
                    return "MakeArabicAvilableForThisMangaSuccessfully";
                }

                logger.LogInformation("Arabic made not available for this manga - تم إلغاء تفعيل العربية لهذه المانجا | MangaId: {MangaId}", id);
                return "MakeArabicNotAvilableForThisMangaSuccessfully";
            }
            catch (Exception exp)
            {
                logger.LogError(exp, "Error toggling Arabic availability - خطأ أثناء تبديل توفر العربية | MangaId: {MangaId}", id);
                return "AnErrorOccurredWhileMakingArabicAvilableOrNotAvilableProcess";
            }
        }

        public async Task<string> MakeEnglishAvailableOrUnAvailableAsync(int id)
        {
            logger.LogInformation("Toggling English availability - تبديل توفر اللغة الإنجليزية | MangaId: {MangaId}", id);

            var manga = await unitOfWork.MangaRepository.GetByIdAsync(id);
            if (manga is null)
            {
                logger.LogWarning("Manga not found for English availability toggle - المانجا غير موجودة لتبديل الإنجليزية | MangaId: {MangaId}", id);
                return "MangaNotFound";
            }

            try
            {
                if (Convert.ToBoolean(manga.EnglishAvilable))
                {
                    manga.EnglishAvilable = false;
                }
                else
                {
                    var chaptersForLang = await context.Chapters
                        .Where(c => c.MangaID.Equals(manga.MangaID) && c.Language.ToLower().Equals("english"))
                        .OrderBy(c => c.ChapterNo)
                        .Select(c => c.ChapterNo)
                        .ToListAsync();

                    bool noGapsAndStartsAtOne = false;
                    if (chaptersForLang.Count > 0)
                    {
                        var ordered = chaptersForLang.OrderBy(n => n).ToList();
                        noGapsAndStartsAtOne = ordered.First() == 1 &&
                                               ordered.Zip(ordered.Skip(1), (a, b) => b - a).All(diff => diff == 1);
                    }
                    manga.EnglishAvilable = noGapsAndStartsAtOne;
                    if (!noGapsAndStartsAtOne)
                    {
                        logger.LogWarning("Cannot make English available due to incomplete chapters - لا يمكن تفعيل الإنجليزية بسبب فصول غير مكتملة | MangaId: {MangaId}", id);
                        return "CanNotMakeEnglishAvialableDueToIncompleteChapters";
                    }
                }

                await unitOfWork.MangaRepository.UpdateAsync(manga);

                if (Convert.ToBoolean(manga.EnglishAvilable))
                {
                    logger.LogInformation("English made available for this manga - تم تفعيل الإنجليزية لهذه المانجا | MangaId: {MangaId}", id);
                    return "MakeEnglishAvilableForThisMangaSuccessfully";
                }

                logger.LogInformation("English made not available for this manga - تم إلغاء تفعيل الإنجليزية لهذه المانجا | MangaId: {MangaId}", id);
                return "MakeEnglishNotAvilableForThisMangaSuccessfully";
            }
            catch (Exception exp)
            {
                logger.LogError(exp, "Error toggling English availability - خطأ أثناء تبديل توفر الإنجليزية | MangaId: {MangaId}", id);
                return "AnErrorOccurredWhileMakingEnglishAvilableOrNotAvilableProcess";
            }
        }

        public async Task<string> ActivateAndDeActivateMangaAsync(int id)
        {
            logger.LogInformation("Toggling manga activation - تبديل حالة تفعيل المانجا | MangaId: {MangaId}", id);

            var manga = await unitOfWork.MangaRepository.GetByIdAsync(id);
            if (manga is null)
            {
                logger.LogWarning("Manga not found for activation/deactivation - المانجا غير موجودة للتفعيل أو التعطيل | MangaId: {MangaId}", id);
                return "MangaNotFound";
            }

            try
            {
                if (manga.IsActive)
                    manga.IsActive = false;
                else
                    manga.IsActive = true;

                await unitOfWork.MangaRepository.UpdateAsync(manga);

                if (manga.IsActive)
                {
                    logger.LogInformation("Manga activated successfully - تم تفعيل المانجا بنجاح | MangaId: {MangaId}", id);
                    return "ActivateMangaSuccessfully";
                }

                logger.LogInformation("Manga deactivated successfully - تم تعطيل المانجا بنجاح | MangaId: {MangaId}", id);
                return "DeActivateMangaSuccessfully";
            }
            catch (Exception exp)
            {
                logger.LogError(exp, "Error in activate/deactivate process - خطأ أثناء عملية التفعيل أو التعطيل | MangaId: {MangaId}", id);
                return "AnErrorOccurredWhileActivatingOrDeActivatingProcess";
            }
        }

        public async Task<(string, PaginatedResult<GetMangaForDashboardResponse>?)> GetMangaForDashboardAsync(string? search, int pageNumber, int pageSize)
        {
            logger.LogInformation("Getting mangas for dashboard - جلب المانجات للوحة التحكم | Search: {Search}, Page: {Page}, Size: {Size}", search, pageNumber, pageSize);

            bool flag = await unitOfWork.MangaRepository.IsAdmin();
            var (message, mangas) = await unitOfWork.MangaRepository.GetMangaForDashboardAsync(search, pageNumber, pageSize, flag);

            logger.LogInformation("GetMangaForDashboardAsync result - نتيجة جلب المانجات للوحة التحكم | Message: {Message}", message);

            return message switch
            {
                "MangaNotFound" => ("MangaNotFound", null),
                "MangaFound" => ("MangaFound", mangas),
                _ => ("MangaNotFound", null)
            };
        }
    }
}