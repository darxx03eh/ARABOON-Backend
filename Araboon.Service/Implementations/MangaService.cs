using Araboon.Data.DTOs.Mangas;
using Araboon.Data.Entities;
using Araboon.Data.Entities.Identity;
using Araboon.Data.Enums;
using Araboon.Data.Response.Mangas.Queries;
using Araboon.Data.Wrappers;
using Araboon.Infrastructure.Data;
using Araboon.Infrastructure.IRepositories;
using Araboon.Service.Interfaces;
using CloudinaryDotNet;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Araboon.Service.Implementations
{
    public class MangaService : IMangaService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ICloudinaryService cloudinaryService;
        private readonly AraboonDbContext context;
        private readonly UserManager<AraboonUser> userManager;

        public MangaService(IUnitOfWork unitOfWork, ICloudinaryService cloudinaryService, AraboonDbContext context,
            UserManager<AraboonUser> userManager)
        {
            this.unitOfWork = unitOfWork;
            this.cloudinaryService = cloudinaryService;
            this.context = context;
            this.userManager = userManager;
        }

        public async Task<(string, IList<HomePageResponse>?, IList<string>?)> GetCategoriesHomePageAsync()
        {
            bool flag = await unitOfWork.MangaRepository.IsAdmin();
            var (message, list, categories) = await unitOfWork.MangaRepository.GetCategoriesHomePageAsync(flag);
            return message switch
            {
                "MangaNotFound" => ("MangaNotFound", null, null),
                "MangaFound" => ("MangaFound", list, categories),
                _ => ("MangaNotFound", null, null)
            };
        }
        public async Task<(string, IList<GetHottestMangasResponse>?)> GetHottestMangasAsync()
        {
            bool flag = await unitOfWork.MangaRepository.IsAdmin();
            var (message, hottestMangas) = await unitOfWork.MangaRepository.GetHottestMangasAsync(flag);
            return message switch
            {
                "MangaNotFound" => ("MangaNotFound", null),
                "MangaFound" => ("MangaFound", hottestMangas),
                _ => ("MangaNotFound", null)
            };
        }
        public async Task<(string, PaginatedResult<GetPaginatedHottestMangaResponse>?)> GetPaginatedHottestMangaAsync(int pageNumber, int pageSize)
        {
            bool flag = await unitOfWork.MangaRepository.IsAdmin();
            var (message, hottestMangas) = await unitOfWork.MangaRepository.GetPaginatedHottestMangaAsync(pageNumber, pageSize, flag);
            return message switch
            {
                "MangaNotFound" => ("MangaNotFound", null),
                "MangaFound" => ("MangaFound", hottestMangas),
                _ => ("MangaNotFound", null)
            };
        }
        public async Task<(string, PaginatedResult<GetMangaByCategoryNameResponse>?)> GetMangaByCategoryNameAsync(string category, int pageNumber, int pageSize)
        {
            bool flag = await unitOfWork.MangaRepository.IsAdmin();
            var (message, mangas) = await unitOfWork.MangaRepository.GetMangaByCategoryNameAsync(category, pageNumber, pageSize, flag);
            return message switch
            {
                "MangaNotFound" => ("MangaNotFound", null),
                "MangaFound" => ("MangaFound", mangas),
                _ => ("MangaNotFound", null)
            };
        }
        public async Task<(string, PaginatedResult<GetMangaByStatusResponse>?)> GetMangaByStatusAsync(int pageNumber, int pageSize, string status, MangaOrderingEnum orderBy, string? filter)
        {
            bool flag = await unitOfWork.MangaRepository.IsAdmin();
            var (message, mangas) = await unitOfWork.MangaRepository.GetMangaByStatusAsync(pageNumber, pageSize, status, orderBy, filter, flag);
            return message switch
            {
                "MangaNotFound" => ("MangaNotFound", null),
                "MangaFound" => ("MangaFound", mangas),
                _ => ("MangaNotFound", null)
            };
        }
        public async Task<(string, Manga?, string?, int?)> GetMangaByIDAsync(int id)
        {
            bool flag = await unitOfWork.MangaRepository.IsAdmin();
            var manga = await unitOfWork.MangaRepository.GetByIdAsync(id);
            if (manga is null)
                return ("MangaNotFound", null, null, null);
            if(flag ? false:!manga.IsActive)
                return ("MangaNotFound", null, null, null);
            var commentCounts = await unitOfWork.MangaRepository.CommentsCountByIdAsync(id);
            return ("MangaFound", manga, manga.MangaNameEn, commentCounts);
        }
        public async Task<(string, PaginatedResult<MangaSearchResponse>?)> SearchAsync(string? search, int pageNumber, int pageSize)
        {
            bool flag = await unitOfWork.MangaRepository.IsAdmin();
            var (message, mangas) = await unitOfWork.MangaRepository.SearchAsync(search, pageNumber, pageSize, flag);
            return message switch
            {
                "MangaNotFound" => ("MangaNotFound", null),
                "MangaFound" => ("MangaFound", mangas),
                _ => ("MangaNotFound", null)
            };
        }
        public async Task<(string, PaginatedResult<GetMangaCommentsResponse>?)> GetMangaCommentsAsync(int id, int pageNumber, int pageSize)
        {
            var (message, comments) = await unitOfWork.MangaRepository.GetMangaCommentsAsync(id, pageNumber, pageSize);
            return message switch
            {
                "CommentsNotFound" => ("CommentsNotFound", null),
                "CommentsFound" => ("CommentsFound", comments),
                _ => ("CommentsNotFound", null)
            };
        }

        public async Task<(string, int?)> GetCommentsCountAsync(int id)
        {
            var (message, commentsCount) = await unitOfWork.CommentRepository.GetCommentsCountForMangaAsync(id);
            return message switch
            {
                "MangaNotFound" => ("MangaNotFound", null),
                "CommentsNotFound" => ("CommentsNotFound", null),
                "CommentsFound" => ("CommentsFound", commentsCount),
                _ => ("MangaNotFound", null)
            };
        }

        public async Task<(string, int?, string?)> AddNewMangaAsync(MangaInfoDTO mangaInfo)
        {
            await using var transaction = await context.Database.BeginTransactionAsync();

            try
            {
                var categories = new List<Category>();
                foreach (var categoryId in mangaInfo.CategoriesIds)
                {
                    var category = await unitOfWork.CategoryRepository.GetByIdAsync(categoryId);
                    if (category is null)
                        return ("CategoryNotFound", null, null);
                    categories.Add(category);
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
                    return ("ThereWasAProblemAddingTheManga", null, null);


                mangaResult.CategoryMangas = categories
                    .Select(c => new CategoryManga { MangaID = mangaResult.MangaID, CategoryID = c.CategoryID })
                    .ToList();

                if (mangaInfo.Image is not null)
                {
                    var url = await UploadMangaImageAsync(mangaInfo.Image, mangaResult.MangaID);
                    if (url is null)
                        return ("AnErrorOccurredWhileAddingTheImageForManga", null, null);
                    mangaResult.MainImage = url;
                }

                await unitOfWork.MangaRepository.UpdateAsync(mangaResult);
                await transaction.CommitAsync();
                return ("MangaAddedSuccessfully", mangaResult.MangaID, mangaResult.MainImage);
            }
            catch
            {
                if (transaction.GetDbTransaction().Connection is not null)
                    await transaction.RollbackAsync();
                return ("AnErrorOccurredWhileAddingTheManga", null, null);
            }
        }
        public async Task<string> UpdateExistMangaAsync(UpdateMangaInfoDTO mangaInfo, int mangaId)
        {
            var manga = await unitOfWork.MangaRepository.GetByIdAsync(mangaId);
            if (manga is null)
                return "MangaNotFound";

            var categories = new List<Category>();
            foreach (var categoryId in mangaInfo.CategoriesIds)
            {
                var category = await unitOfWork.CategoryRepository.GetByIdAsync(categoryId);
                if (category is null)
                    return "CategoryNotFound";
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
                return "MangaUpdatingSuccessfully";
            }
            catch (Exception exp)
            {
                if(transaction.GetDbTransaction().Connection is not null)
                    await transaction.RollbackAsync();
                return "AnErrorOccurredWhileUpdatingTheManga";
            }
        }
        private async Task<string?> UploadMangaImageAsync(IFormFile image, int mangaId)
        {
            try
            {
                var guidPart = Guid.NewGuid().ToString("N").Substring(0, 12);
                var datePart = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
                var id = $"{guidPart}-{datePart}";

                using var stream = image.OpenReadStream();
                var folderName = $"ARABOON/Mangas/{mangaId}/img";
                return await cloudinaryService.UploadFileAsync(stream, folderName, id);
            }
            catch
            {
                return null;
            }
        }

        public async Task<string> DeleteMangaAsync(int id)
        {
            var manga = await unitOfWork.MangaRepository.GetByIdAsync(id);
            if (manga is null)
                return "MangaNotFound";

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

                await unitOfWork.MangaRepository.DeleteAsync(manga);
                await transaction.CommitAsync();
                return "MangaDeletedSuccessfully";
            }
            catch (Exception exp)
            {
                if (transaction.GetDbTransaction().Connection is not null)
                    await transaction.RollbackAsync();
                return "AnErrorOccurredWhileDeletingTheManga";
            }
        }

        public async Task<string> DeleteMangaImageAsync(int id)
        {
            var manga = await unitOfWork.MangaRepository.GetByIdAsync(id);
            if (manga is null)
                return "MangaNotFound";

            var url = manga.MainImage;
            if (string.IsNullOrWhiteSpace(url))
                return "ThereIsNoImageToDelete";
            try
            {
                var cloudinaryResult = cloudinaryService.DeleteFileAsync(url);
                if (cloudinaryResult.Equals("FailedToDeleteImageFromCloudinary"))
                    return "FailedToDeleteImageFromCloudinary";
                manga.MainImage = null;
                await unitOfWork.MangaRepository.UpdateAsync(manga);
                return "ImageHasBeenSuccessfullyDeleted";
            }
            catch (Exception exp)
            {
                return "AnErrorOccurredWhileDeletingTheImage";
            }
        }

        public async Task<(string, string?)> UploadMangaImageAsync(int id, IFormFile image)
        {
            var manga = await unitOfWork.MangaRepository.GetByIdAsync(id);
            if (manga is null)
                return ("MangaNotFound", null);

            await using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                var originalUrl = manga.MainImage;
                if (!string.IsNullOrWhiteSpace(originalUrl))
                {
                    var cloudinaryResult = await cloudinaryService.DeleteFileAsync(originalUrl);
                    if (cloudinaryResult.Equals("FailedToDeleteImageFromCloudinary"))
                        return ("FailedToDeleteOldImageFromCloudinary", null);
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
                return ("TheImageHasBeenChangedSuccessfully", manga.MainImage);
            }
            catch (Exception exp)
            {
                if (transaction.GetDbTransaction().Connection is not null)
                    await transaction.RollbackAsync();
                return ("AnErrorOccurredWhileProcessingImageModificationRequest", null);
            }
        }

        public async Task<string> MakeArabicAvailableAsync(int id)
        {
            var manga = await unitOfWork.MangaRepository.GetByIdAsync(id);
            if (manga is null)
                return "MangaNotFound";

            if (Convert.ToBoolean(manga.ArabicAvailable))
                return "ArabicAvailableForThisMangaAlready";

            try
            {
                manga.ArabicAvailable = true;
                await unitOfWork.MangaRepository.UpdateAsync(manga);
                return "MakeArabicAvilableForThisMangaSuccessfully";
            }
            catch (Exception exp)
            {
                return "AnErrorOccurredWhileMakingArabicAvilableForThisManga";
            }
        }

        public async Task<string> MakeArabicUnAvailableAsync(int id)
        {
            var manga = await unitOfWork.MangaRepository.GetByIdAsync(id);
            if (manga is null)
                return "MangaNotFound";

            if (!Convert.ToBoolean(manga.ArabicAvailable))
                return "ArabicNotAvailableForThisMangaAlready";

            try
            {
                manga.ArabicAvailable = false;
                await unitOfWork.MangaRepository.UpdateAsync(manga);
                return "MakeArabicNotAvilableForThisMangaSuccessfully";
            }
            catch (Exception exp)
            {
                return "AnErrorOccurredWhileMakingArabicNotAvilableForThisManga";
            }
        }

        public async Task<string> MakeEnglishAvailableAsync(int id)
        {
            var manga = await unitOfWork.MangaRepository.GetByIdAsync(id);
            if (manga is null)
                return "MangaNotFound";

            if (Convert.ToBoolean(manga.EnglishAvilable))
                return "EnglishAvailableForThisMangaAlready";

            try
            {
                manga.EnglishAvilable = true;
                await unitOfWork.MangaRepository.UpdateAsync(manga);
                return "MakeEnglishAvilableForThisMangaSuccessfully";
            }
            catch (Exception exp)
            {
                return "AnErrorOccurredWhileMakingEnglishAvilableForThisManga";
            }
        }

        public async Task<string> MakeEnglishUnAvailableAsync(int id)
        {
            var manga = await unitOfWork.MangaRepository.GetByIdAsync(id);
            if (manga is null)
                return "MangaNotFound";

            if (!Convert.ToBoolean(manga.EnglishAvilable))
                return "EnglishNotAvailableForThisMangaAlready";

            try
            {
                manga.EnglishAvilable = false;
                await unitOfWork.MangaRepository.UpdateAsync(manga);
                return "MakeEnglishNotAvilableForThisMangaSuccessfully";
            }
            catch (Exception exp)
            {
                return "AnErrorOccurredWhileMakingEnglishNotAvilableForThisManga";
            }
        }

        public async Task<string> ActivateMangaAsync(int id)
        {
            var manga = await unitOfWork.MangaRepository.GetByIdAsync(id);
            if (manga is null)
                return "MangaNotFound";

            if (manga.IsActive)
                return "MangaAlreadyActive";

            try
            {
                manga.IsActive = true;
                await unitOfWork.MangaRepository.UpdateAsync(manga);
                return "ActivateMangaSuccessfully";
            }
            catch (Exception exp)
            {
                return "AnErrorOccurredWhileActivateThisManga";
            }
        }

        public async Task<string> DeActivateMangaAsync(int id)
        {
            var manga = await unitOfWork.MangaRepository.GetByIdAsync(id);
            if (manga is null)
                return "MangaNotFound";

            if (!manga.IsActive)
                return "MangaAlreadyDeActive";

            try
            {
                manga.IsActive = false;
                await unitOfWork.MangaRepository.UpdateAsync(manga);
                return "DeActivateMangaSuccessfully";
            }
            catch (Exception exp)
            {
                return "AnErrorOccurredWhileDeActivateThisManga";
            }
        }

        
    }
}
