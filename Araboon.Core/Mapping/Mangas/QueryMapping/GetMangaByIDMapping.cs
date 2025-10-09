using Araboon.Data.Entities;
using Araboon.Data.Helpers.Resolvers.Mangas;
using Araboon.Data.Helpers.Resolvers.MangasResolver;
using Araboon.Data.Response.Mangas.Queries;
using Araboon.Infrastructure.Commons;
using Araboon.Infrastructure.Resolvers.MangasResolver;

namespace Araboon.Core.Mapping.Mangas
{
    public partial class MangaProfile
    {
        void GetMangaByIDMapping()
        {
            CreateMap<Manga, GetMangaByIDResponse>()
                .ForMember(to => to.MangaId, from => from.MapFrom(src => src.MangaID))
                .ForMember(to => to.MangaName, from => from.MapFrom(src => TransableEntity.GetTransable(src.MangaNameEn, src.MangaNameAr)))
                .ForMember(to => to.Author, from => from.MapFrom(src => TransableEntity.GetTransable(src.AuthorEn, src.AuthorAr)))
                .ForMember(to => to.MangaImageUrl, from => from.MapFrom(src => src.MainImage))
                .ForMember(to => to.Categories, from => from.MapFrom(src => src.CategoryMangas.Select(c =>
                    TransableEntity.GetTransable(c.Category.CategoryNameEn, c.Category.CategoryNameAr)
                    )))
                .ForMember(to => to.IsArabicAvailable, from => from.MapFrom(src => src.ArabicAvailable))
                .ForMember(to => to.IsEnglishAvailable, from => from.MapFrom(src => src.EnglishAvilable))
                .ForMember(to => to.Status, from => from.MapFrom(src => TransableEntity.GetTransable(src.StatusEn, src.StatusAr)))
                .ForMember(to => to.Type, from => from.MapFrom(src => TransableEntity.GetTransable(src.TypeEn, src.TypeAr)))
                .ForMember(to => to.Description, from => from.MapFrom(src => TransableEntity.GetTransable(src.DescriptionEn, src.DescriptionAr)))
                .ForMember(to => to.PublishedOn, from => from.MapFrom<MangaDateFormatResolver>())
                .ForMember(to => to.UpdatedOn, from => from.MapFrom<MangaDateFormatResolver>())
                .ForMember(to => to.Rate, from => from.MapFrom(src => src.Rate))
                .ForMember(to => to.IsFavorite, from => from.MapFrom<IsFavoriteResolver>())
                .ForMember(to => to.IsCompletedReading, from => from.MapFrom<IsCompletedReadingResolver>())
                .ForMember(to => to.IsCurrentlyReading, from => from.MapFrom<IsCurrentlyReadingResolver>())
                .ForMember(to => to.IsReadingLater, from => from.MapFrom<IsReadingLaterResolver>())
                .ForMember(to => to.IsNotification, from => from.MapFrom<IsNotificationResolver>())
                .ForMember(to => to.CommentsCount, from => from.MapFrom<CommentsCountResolver>());
        }
    }
}
