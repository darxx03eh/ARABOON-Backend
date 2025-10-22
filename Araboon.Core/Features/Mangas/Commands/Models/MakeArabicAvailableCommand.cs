using Araboon.Core.Bases;
using MediatR;

namespace Araboon.Core.Features.Mangas.Commands.Models
{
    public class MakeArabicAvailableCommand : IRequest<ApiResponse>
    {
        public int Id { get; set; }
        public MakeArabicAvailableCommand(int id) => Id = id;
    }
}
