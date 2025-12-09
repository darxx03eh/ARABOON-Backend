using Araboon.Core.Bases;
using MediatR;

namespace Araboon.Core.Features.Chapters.Queries.Models
{
    public class GetChaptersForSpecificMangaByLanguageQuery : IRequest<ApiResponse>
    {
        public int MangaID { get; set; }
        public string Language { get; set; }
    }
}
