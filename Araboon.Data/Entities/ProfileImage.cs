using Araboon.Data.Entities.Identity;

namespace Araboon.Data.Entities
{
    public class ProfileImage
    {
        public int ID { get; set; }
        public string? OriginalImage { get; set; }
        public decimal X { get; set; }
        public decimal Y { get; set; }
        public decimal Scale { get; set; }
        public decimal Rotate { get; set; }
        public int UserID { get; set; }
        public virtual AraboonUser? User { get; set; }
    }
}
