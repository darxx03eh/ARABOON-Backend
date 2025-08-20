using Araboon.Core.Bases;
using Araboon.Data.Enums;
using MediatR;

namespace Araboon.Core.Features.Mangas.Queries.Models
{
    public class GetMangaByStatusQuery : IRequest<ApiResponse>
    {
        public String Status { get; set; }
        public Int32 PageNumber { get; set; }
        public Int32 PageSize { get; set; }
        public MangaOrderingEnum OrderBy { get; set; }
        public String? Filter { get; set; }
    }
}
