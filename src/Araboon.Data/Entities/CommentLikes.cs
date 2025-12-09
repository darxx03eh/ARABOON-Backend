using Araboon.Data.Entities.Identity;

namespace Araboon.Data.Entities
{
    public class CommentLikes
    {
        public int UserId { get; set; }
        public int CommentId { get; set; }

        public virtual AraboonUser? User { get; set; }
        public virtual Comment? Comment { get; set; }
    }
}
