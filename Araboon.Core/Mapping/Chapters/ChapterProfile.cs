using AutoMapper;
namespace Araboon.Core.Mapping.Chapters
{
    public partial class ChapterProfile : Profile
    {
        public ChapterProfile()
        {
            GetChaptersByLanguageMapping();
        }
    }
}
