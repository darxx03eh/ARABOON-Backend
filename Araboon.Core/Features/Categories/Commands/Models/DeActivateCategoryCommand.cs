using Araboon.Core.Bases;
using MediatR;

namespace Araboon.Core.Features.Categories.Commands.Models
{
    public class DeActivateCategoryCommand : IRequest<ApiResponse>
    {
        public int Id { get; set; }
        public DeActivateCategoryCommand(int id) => Id = id;
    }
}
