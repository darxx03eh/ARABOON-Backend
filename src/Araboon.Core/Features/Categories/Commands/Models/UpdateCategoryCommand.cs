using Araboon.Core.Bases;
using MediatR;

namespace Araboon.Core.Features.Categories.Commands.Models
{
    public class UpdateCategoryCommand : IRequest<ApiResponse>
    {
        public int Id { get; set; }
        public string CategoryNameEn { get; set; }
        public string CategoryNameAr { get; set; }
    }
}
