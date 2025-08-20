using Araboon.Data.Entities;
using Araboon.Data.Response.Categories.Queries;
using Araboon.Infrastructure.Commons;

namespace Araboon.Core.Mapping.Categories
{
    public partial class CategoryProfile
    {
        public void GetCategoriesMapping()
        {
            CreateMap<Category, CategoriesResponse>()
                 .ForMember(to => to.En, from => from.MapFrom(src => src.CategoryNameEn))
                 .ForMember(to => to.Ar, from => from.MapFrom(src => src.CategoryNameAr));
        }
    }
}
