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
            if (flag ? false : !manga.IsActive)
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

        public async Task<(string, GetMangaForDashboardResponse?)> AddNewMangaAsync(MangaInfoDTO mangaInfo)
        {
            await using var transaction = await context.Database.BeginTransactionAsync();

            try
            {
                var categories = new List<Category>();
                if(mangaInfo.CategoriesIds is not null)
                {
                    foreach (var categoryId in mangaInfo.CategoriesIds)
                    {
                        var category = await unitOfWork.CategoryRepository.GetByIdAsync(categoryId);
                        if (category is null)
                            return ("CategoryNotFound", null);
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
                    return ("ThereWasAProblemAddingTheManga", null);


                mangaResult.CategoryMangas = categories
                    .Select(c => new CategoryManga { MangaID = mangaResult.MangaID, CategoryID = c.CategoryID })
                    .ToList();

                if (mangaInfo.Image is not null)
                {
                    var url = await UploadMangaImageAsync(mangaInfo.Image, mangaResult.MangaID);
                    if (url is null)
                        return ("AnErrorOccurredWhileAddingTheImageForManga", null);
                    mangaResult.MainImage = url;
                }

                await unitOfWork.MangaRepository.UpdateAsync(mangaResult);
                await transaction.CommitAsync();
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
            catch
            {
                if (transaction.GetDbTransaction().Connection is not null)
                    await transaction.RollbackAsync();
                return ("AnErrorOccurredWhileAddingTheManga", null);
            }
        }
        public async Task<(string, GetMangaForDashboardResponse?)> UpdateExistMangaAsync(UpdateMangaInfoDTO mangaInfo, int mangaId)
        {
            var manga = await unitOfWork.MangaRepository.GetByIdAsync(mangaId);
            if (manga is null)
                return ("MangaNotFound", null);

            var categories = new List<Category>();
            foreach (var categoryId in mangaInfo.CategoriesIds)
            {
                var category = await unitOfWork.CategoryRepository.GetByIdAsync(categoryId);
                if (category is null)
                    return ("CategoryNotFound", null);
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
                if (transaction.GetDbTransaction().Connection is not null)
                    await transaction.RollbackAsync();
                return ("AnErrorOccurredWhileUpdatingTheManga", null);
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

        public async Task<string> MakeArabicAvailableOrUnAvailableAsync(int id)
        {
            var manga = await unitOfWork.MangaRepository.GetByIdAsync(id);
            if (manga is null)
                return "MangaNotFound";
            try
            {
                if (Convert.ToBoolean(manga.ArabicAvailable))
                    manga.ArabicAvailable = false;
                else manga.ArabicAvailable = true;
                await unitOfWork.MangaRepository.UpdateAsync(manga);
                if(Convert.ToBoolean(manga.ArabicAvailable)) return "MakeArabicAvilableForThisMangaSuccessfully";
                return "MakeArabicNotAvilableForThisMangaSuccessfully";
            }
            catch (Exception exp)
            {
                return "AnErrorOccurredWhileMakingArabicAvilableOrNotAvilableProcess";
            }
        }
        public async Task<string> MakeEnglishAvailableOrUnAvailableAsync(int id)
        {
            var manga = await unitOfWork.MangaRepository.GetByIdAsync(id);
            if (manga is null)
                return "MangaNotFound";
            try
            {
                if (Convert.ToBoolean(manga.EnglishAvilable))
                    manga.EnglishAvilable = false;
                else manga.EnglishAvilable = true;
                await unitOfWork.MangaRepository.UpdateAsync(manga);
                if(Convert.ToBoolean(manga.EnglishAvilable)) return "MakeEnglishAvilableForThisMangaSuccessfully";
                return "MakeEnglishNotAvilableForThisMangaSuccessfully";
            }
            catch (Exception exp)
            {
                return "AnErrorOccurredWhileMakingEnglishAvilableOrNotAvilableProcess";
            }
        }
        public async Task<string> ActivateAndDeActivateMangaAsync(int id)
        {
            var manga = await unitOfWork.MangaRepository.GetByIdAsync(id);
            if (manga is null)
                return "MangaNotFound";
            try
            {
                if (manga.IsActive)
                    manga.IsActive = false;
                else manga.IsActive = true;
                await unitOfWork.MangaRepository.UpdateAsync(manga);
                if(manga.IsActive) return "ActivateMangaSuccessfully";
                return "DeActivateMangaSuccessfully";
            }
            catch (Exception exp)
            {
                return "AnErrorOccurredWhileActivatingOrDeActivatingProcess";
            }
        }

        public async Task<(string, PaginatedResult<GetMangaForDashboardResponse>?)> GetMangaForDashboardAsync(string? search, int pageNumber, int pageSize)
        {
            bool flag = await unitOfWork.MangaRepository.IsAdmin();
            var (message, mangas) = await unitOfWork.MangaRepository.GetMangaForDashboardAsync(search, pageNumber, pageSize, flag);
            return message switch
            {
                "MangaNotFound" => ("MangaNotFound", null),
                "MangaFound" => ("MangaFound", mangas),
                _ => ("MangaNotFound", null)
            };
        }
    }
}
