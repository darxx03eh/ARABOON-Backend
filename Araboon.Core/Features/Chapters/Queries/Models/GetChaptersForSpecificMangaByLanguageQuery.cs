using Araboon.Core.Bases;
using MediatR;

namespace Araboon.Core.Features.Chapters.Queries.Models
{
    public class GetChaptersForSpecificMangaByLanguageQuery : IRequest<ApiResponse>
    {
        public Int32 MangaID { get; set; }
        public String Language { get; set; }
    }
}
