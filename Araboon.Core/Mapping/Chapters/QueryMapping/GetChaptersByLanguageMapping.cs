using Araboon.Data.Entities;
using Araboon.Data.Helpers.Resolvers.ChaptersResolver;
using Araboon.Data.Response.Chapters.Queries;
using Araboon.Infrastructure.Commons;
using Araboon.Infrastructure.Resolvers.ChaptersResolver;

namespace Araboon.Core.Mapping.Chapters
{
    public partial class ChapterProfile
    {
        public void GetChaptersByLanguageMapping()
        {
            CreateMap<Chapter, ChaptersResponse>()
                .ForMember(to => to.ChapterID, from => from.MapFrom(src => src.ChapterID))
                .ForMember(to => to.Title, from => from.MapFrom(src => $"#{src.ChapterNo.ToString("D3")}"))
                .ForMember(to => to.ChapterImageUrl, from => from.MapFrom(src => src.ImageUrl))
                .ForMember(to => to.ChapterTitle, from => from.MapFrom(src => TransableEntity.GetTransable(src.EnglishChapterTitle, src.ArabicChapterTitle)))
                .ForMember(to => to.ChapterTitleAr, from => from.MapFrom(src => src.ArabicChapterTitle))
                .ForMember(to => to.ChapterTitleEn, from => from.MapFrom(src => src.EnglishChapterTitle))
                .ForMember(to => to.ReleasedOn, from => from.MapFrom<ChapterDateFormatResolver>())
                .ForMember(to => to.IsView, from => from.MapFrom<IsViewResolver>())
                .ForMember(to => to.IsArabic, from => from.MapFrom<ChapterIsArabicResolver>());
        }
    }
}
