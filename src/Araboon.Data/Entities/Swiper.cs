namespace Araboon.Data.Entities
{
    public class Swiper
    {
        public int SwiperId { get; set; }
        public bool IsActive { get; set; }
        public string? ImageUrl { get; set; }
        public string? NoteEn {  get; set; }
        public string? NoteAr {  get; set; }
        public string Link { get; set; }
        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
