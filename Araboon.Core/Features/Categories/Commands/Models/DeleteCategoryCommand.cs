using Araboon.Core.Bases;
using MediatR;

namespace Araboon.Core.Features.Categories.Commands.Models
{
    public class DeleteCategoryCommand : IRequest<ApiResponse>
    {
        public int Id { get; set; }
        public DeleteCategoryCommand(int id) => Id = id;
    }
}
