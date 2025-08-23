using Araboon.Data.Helpers;
using Microsoft.AspNetCore.Identity;

namespace Araboon.Data.Entities.Identity
{
    public class AraboonUser : IdentityUser<int>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? ProfileImage { get; set; }
        public string? CoverImage { get; set; }
        public string? Bio { get; set; }
        private string? code;
        public string? Code
        {
            get => code is null ? null : EncryptionHelper.Decrypt(code);
            set => code = value is null ? null : EncryptionHelper.Encrypt(value);
        }
        private string? forgetPasswordToken;
        public string? ForgetPasswordToken
        {
            get => forgetPasswordToken is null ? null : EncryptionHelper.Decrypt(forgetPasswordToken);
            set => forgetPasswordToken = value is null ? null : EncryptionHelper.Encrypt(value);
        }
        public DateTime? CodeExpiryDate { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public virtual ICollection<UserRefreshToken>? UserRefreshTokens { get; set; } = new HashSet<UserRefreshToken>();
        public virtual ICollection<Favorite>? Favorites { get; set; } = new HashSet<Favorite>();
        public virtual ICollection<CompletedReads>? CompletedReads { get; set; } = new HashSet<CompletedReads>();
        public virtual ICollection<CurrentlyReading>? CurrentlyReadings { get; set; } = new HashSet<CurrentlyReading>();
        public virtual ICollection<ReadingLater>? ReadingLaters { get; set; } = new HashSet<ReadingLater>();
        public virtual ICollection<Notifications>? Notifications { get; set; } = new HashSet<Notifications>();
        public virtual ICollection<Comment>? Comments { get; set; } = new HashSet<Comment>();
        public virtual ICollection<Reply>? Replies { get; set; } = new HashSet<Reply>();
        public virtual ICollection<ChapterView>? ChapterViews { get; set; } = new HashSet<ChapterView>();
    }
}
