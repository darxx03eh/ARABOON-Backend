using Araboon.Core.Bases;
using MediatR;

namespace Araboon.Core.Features.Mangas.Commands.Models
{
    public class MakeArabicUnAvailableCommand : IRequest<ApiResponse>
    {
        public int Id { get; set; }
        public MakeArabicUnAvailableCommand(int id) => Id = id;
    }
}
