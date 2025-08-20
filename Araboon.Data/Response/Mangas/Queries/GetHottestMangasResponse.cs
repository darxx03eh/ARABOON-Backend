namespace Araboon.Data.Response.Mangas.Queries
{
    public class GetHottestMangasResponse
    {
        public Int32 MangaID { get; set; }
        public String? MangaName { get; set; }
        public String? MangaImageUrl { get; set; }
        public String? AuthorName { get; set; }
        public Int32? PopularityScore { get; set; }
    }
}
