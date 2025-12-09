using Araboon.Core.Bases;
using MediatR;

namespace Araboon.Core.Features.Categories.Commands.Models
{
    public class AddNewCategoryCommand : IRequest<ApiResponse>
    {
        public string CategoryNameEn {  get; set; }
        public string CategoryNameAr { get; set; }
    }
}
