using Araboon.Core.Bases;
using Araboon.Data.Response.Users.Queries;
using MediatR;

namespace Araboon.Core.Features.Users.Commands.Models
{
    public class ChangeCroppedDataCommand : IRequest<ApiResponse>
    {
        public CropData CropData { get; set; }
    }
}
