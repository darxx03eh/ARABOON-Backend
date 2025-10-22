using Araboon.Data.DTOs.Mangas;
using Araboon.Data.Entities;
using Araboon.Data.Enums;
using Araboon.Data.Response.Mangas.Queries;
using Araboon.Data.Wrappers;
using Araboon.Infrastructure.Data;
using Araboon.Infrastructure.IRepositories;
using Araboon.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Storage;
using static System.Net.Mime.MediaTypeNames;

namespace Araboon.Service.Implementations
{
    public class MangaService : IMangaService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ICloudinaryService cloudinaryService;
        private readonly AraboonDbContext context;

        public MangaService(IUnitOfWork unitOfWork, ICloudinaryService cloudinaryService, AraboonDbContext context)
        {
            this.unitOfWork = unitOfWork;
            this.cloudinaryService = cloudinaryService;
            this.context = context;
        }

        public async Task<(string, IList<HomePageResponse>?, IList<string>?)> GetCategoriesHomePageAsync()
        {
            var (message, list, categories) = await unitOfWork.MangaRepository.GetCategoriesHomePageAsync();
            return message switch
            {
                "MangaNotFound" => ("MangaNotFound", null, null),
                "MangaFound" => ("MangaFound", list, categories),
                _ => ("MangaNotFound", null, null)
            };
        }
        public async Task<(string, IList<GetHottestMangasResponse>?)> GetHottestMangasAsync()
        {
            var (message, hottestMangas) = await unitOfWork.MangaRepository.GetHottestMangasAsync();
            return message switch
            {
                "MangaNotFound" => ("MangaNotFound", null),
                "MangaFound" => ("MangaFound", hottestMangas),
                _ => ("MangaNotFound", null)
            };
        }
        public async Task<(string, PaginatedResult<GetPaginatedHottestMangaResponse>?)> GetPaginatedHottestMangaAsync(int pageNumber, int pageSize)
        {
            var (message, hottestMangas) = await unitOfWork.MangaRepository.GetPaginatedHottestMangaAsync(pageNumber, pageSize);
            return message switch
            {
                "MangaNotFound" => ("MangaNotFound", null),
                "MangaFound" => ("MangaFound", hottestMangas),
                _ => ("MangaNotFound", null)
            };
        }
        public async Task<(string, PaginatedResult<GetMangaByCategoryNameResponse>?)> GetMangaByCategoryNameAsync(string category, int pageNumber, int pageSize)
        {
            var (message, mangas) = await unitOfWork.MangaRepository.GetMangaByCategoryNameAsync(category, pageNumber, pageSize);
            return message switch
            {
                "MangaNotFound" => ("MangaNotFound", null),
                "MangaFound" => ("MangaFound", mangas),
                _ => ("MangaNotFound", null)
            };
        }
        public async Task<(string, PaginatedResult<GetMangaByStatusResponse>?)> GetMangaByStatusAsync(int pageNumber, int pageSize, string status, MangaOrderingEnum orderBy, string? filter)
        {
            var (message, mangas) = await unitOfWork.MangaRepository.GetMangaByStatusAsync(pageNumber, pageSize, status, orderBy, filter);
            return message switch
            {
                "MangaNotFound" => ("MangaNotFound", null),
                "MangaFound" => ("MangaFound", mangas),
                _ => ("MangaNotFound", null)
            };
        }
        public async Task<(string, Manga?, string?, int?)> GetMangaByIDAsync(int id)
        {
            var manga = await unitOfWork.MangaRepository.GetByIdAsync(id);
            if (manga is null)
                return ("MangaNotFound", null, null, null);
            var commentCounts = await unitOfWork.MangaRepository.CommentsCountByIdAsync(id);
            return ("MangaFound", manga, manga.MangaNameEn, commentCounts);
        }
        public async Task<(string, PaginatedResult<MangaSearchResponse>?)> SearchAsync(string? search, int pageNumber, int pageSize)
        {
            var (message, mangas) = await unitOfWork.MangaRepository.SearchAsync(search, pageNumber, pageSize);
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

        public async Task<(string, int?, string?)> AddNewMangaCommandAsync(MangaInfoDTO mangaInfo)
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
    }
}
