using Araboon.Data.Response.Users.Queries;

namespace Araboon.Data.Response.Mangas.Queries
{
    public class GetMangaCommentsResponse
    {
        public User User { get; set; }
    }
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ProfileImage? ProfileImage { get; set; }
        public UserComment? Comment { get; set; }
    }
    public class UserComment
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public string Since { get; set; }
        public int? Likes { get; set; }
        public bool IsLike { get; set; }
    }
}
