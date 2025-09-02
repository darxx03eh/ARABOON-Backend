namespace Araboon.Data.Response.Mangas.Queries
{
    public class HomePageResponse
    {
        public CategoryResponse? Category { get; set; }
        public IList<GetCategoriesHomePageResponse>? Mangas { get; set; }
    }
    public class CategoryResponse
    {
        public string En { get; set; }
        public string Ar { get; set; }
    }
}
