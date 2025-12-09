namespace Araboon.Data.Response.Mangas.Queries
{
    public class GetHottestMangasResponse
    {
        public int MangaID { get; set; }
        public string? MangaName { get; set; }
        public string? MangaImageUrl { get; set; }
        public string? AuthorName { get; set; }
        public int? PopularityScore { get; set; }
    }
}
