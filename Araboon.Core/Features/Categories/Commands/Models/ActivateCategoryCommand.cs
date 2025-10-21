using Araboon.Core.Bases;
using MediatR;

namespace Araboon.Core.Features.Categories.Commands.Models
{
    public class ActivateCategoryCommand : IRequest<ApiResponse>
    {
        public int Id { get; set; }
        public ActivateCategoryCommand(int id) => Id = id;
    }
}
