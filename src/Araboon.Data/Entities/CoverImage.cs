using Araboon.Data.Entities.Identity;

namespace Araboon.Data.Entities
{
    public class CoverImage
    {
        public int ID { get; set; }
        public string? OriginalImage { get; set; }
        public string? CroppedImage { get; set; }
        public int UserID { get; set; }
        public virtual AraboonUser? User { get; set; }
    }
}
