using Araboon.Core.Bases;
using MediatR;

namespace Araboon.Core.Features.Mangas.Commands.Models
{
    public class IncreaseMangasViewsByOneCommand : IRequest<ApiResponse>
    {
        public int Id { get; set; }

        public IncreaseMangasViewsByOneCommand(int id) => Id = id;
    }
}