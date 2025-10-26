namespace Araboon.Data.DTOs.Mangas
{
    public class UpdateMangaInfoDTO
    {
        public string MangaNameEn { get; set; }
        public string MangaNameAr { get; set; }
        public string StatusEn { get; set; }
        public string StatusAr { get; set; }
        public string? AuthorEn { get; set; }
        public string? AuthorAr { get; set; }
        public string TypeEn { get; set; }
        public string TypeAr { get; set; }
        public string? DescriptionEn { get; set; }
        public string? DescriptionAr { get; set; }
        public IList<int>? CategoriesIds { get; set; }
    }
}
