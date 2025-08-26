namespace Araboon.Data.Response.Users.Queries
{
    public class UserProfileResponse
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public CoverImage? CoverImage { get; set; }
        public ProfileImage? ProfileImage { get; set; }
        public string JoinDate { get; set; }
        public string Role { get; set; }
        public bool IsActive { get; set; }
        public string? Bio { get; set; }
        public Library? Library { get; set; }
        public IList<FavoritesCategory>? FavoritesCategories { get; set; }
    }
    public class Library
    {
        public int? FavoritesCount { get; set; }
        public int? CompletedReadsCount { get; set; }
        public int? CurrentlyReadingCount { get; set; }
        public int? ReadingLatersCount { get; set; }
    }
    public class FavoritesCategory
    {
        public string Category { get; set; }
        public int Count { get; set; }
    }
    public class CoverImage
    {
        public string? OriginalImage { get; set; }
        public string? CroppedImage { get; set; }
    }
    public class ProfileImage
    {
        public string? OriginalImage { get; set; }
        public CropData CropData { get; set; }
    }
    public class CropData
    {
        public Position Position { get; set; }
        public decimal Scale { get; set; }
        public decimal Rotate { get; set; }
    }
    public class Position
    {
        public decimal X { get; set; }
        public decimal Y { get; set; }
    }
}
