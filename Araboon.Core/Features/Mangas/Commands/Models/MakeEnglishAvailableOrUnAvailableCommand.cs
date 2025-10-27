using Araboon.Core.Bases;
using MediatR;

namespace Araboon.Core.Features.Mangas.Commands.Models
{
    public class MakeEnglishAvailableOrUnAvailableCommand : IRequest<ApiResponse>
    {
        public int Id { get; set; }
        public MakeEnglishAvailableOrUnAvailableCommand(int id) => Id = id;
    }
}
