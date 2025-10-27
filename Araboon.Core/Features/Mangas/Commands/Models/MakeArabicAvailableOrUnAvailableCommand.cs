using Araboon.Core.Bases;
using MediatR;

namespace Araboon.Core.Features.Mangas.Commands.Models
{
    public class MakeArabicAvailableOrUnAvailableCommand : IRequest<ApiResponse>
    {
        public int Id { get; set; }
        public MakeArabicAvailableOrUnAvailableCommand(int id) => Id = id;
    }
}
