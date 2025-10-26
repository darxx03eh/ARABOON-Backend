using Microsoft.AspNetCore.Http;

namespace Araboon.Data.DTOs.Mangas
{
    public class MangaInfoDTO : UpdateMangaInfoDTO
    {
        public IFormFile? Image { get; set; }
    }
}
