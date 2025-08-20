using AutoMapper;

namespace Araboon.Core.Mapping.Categories
{
    public partial class CategoryProfile : Profile
    {
        public CategoryProfile()
        {
            GetCategoriesMapping();
        }
    }
}
