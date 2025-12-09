using Araboon.Core.Bases;
using MediatR;

namespace Araboon.Core.Features.ChapterImages.Queries.Models
{
    public class GetChapterImagesQuery : IRequest<ApiResponse>
    {
        public int MangaId { get; set; }
        public int ChapterNo { get; set; }
        public string Language {  get; set; }
    }
}
