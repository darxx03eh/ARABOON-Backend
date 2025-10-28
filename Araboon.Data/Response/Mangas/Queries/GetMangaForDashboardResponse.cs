using Araboon.Data.Response.Categories.Queries;

namespace Araboon.Data.Response.Mangas.Queries
{
    public class GetMangaForDashboardResponse : MangaSearchResponse
    {
        public MangaName Name { get; set; }
        public Description Description { get; set; }
        public Author Author { get; set; }
        public Type Type { get; set; }
        public Status Status { get; set; }
        public IList<CategoriesResponse> Categories { get; set; }
    }
    public class MangaName
    {
        public string En {  get; set; }
        public string Ar {  get; set; }
    }
    public class Description : MangaName { }
    public class Author : MangaName { }
    public class Type : MangaName { }
    public class Status : MangaName { }
}
