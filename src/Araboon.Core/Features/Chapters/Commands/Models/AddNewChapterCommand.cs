using Araboon.Core.Bases;
using Araboon.Data.DTOs.Chapters;
using MediatR;

namespace Araboon.Core.Features.Chapters.Commands.Models
{
    public class AddNewChapterCommand : ChapterInfoDTO, IRequest<ApiResponse>
    {

    }
}
