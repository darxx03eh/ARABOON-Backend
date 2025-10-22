using Araboon.Core.Bases;
using MediatR;

namespace Araboon.Core.Features.Mangas.Commands.Models
{
    public class MakeEnglishAvailableCommand : IRequest<ApiResponse>
    {
        public int Id { get; set; }
        public MakeEnglishAvailableCommand(int id) => Id = id;
    }
}
