using Araboon.Data.Entities.Identity;

namespace Araboon.Data.Entities
{
    public class ReplyLikes
    {
        public int UserId { get; set; }
        public int ReplyId { get; set; }

        public virtual AraboonUser? User { get; set; }
        public virtual Reply? Reply { get; set; }
    }
}
