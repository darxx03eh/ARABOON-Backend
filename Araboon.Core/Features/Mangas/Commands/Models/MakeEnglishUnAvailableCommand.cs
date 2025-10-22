using Araboon.Core.Bases;
using MediatR;

namespace Araboon.Core.Features.Mangas.Commands.Models
{
    public class MakeEnglishUnAvailableCommand : IRequest<ApiResponse>
    {
        public int Id { get; set; }
        public MakeEnglishUnAvailableCommand(int id) => Id = id;
    }
}
