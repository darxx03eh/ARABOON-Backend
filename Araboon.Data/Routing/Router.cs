namespace Araboon.Data.Routing
{
    public static class Router
    {
        public const string singleRoute = "/{id}";
        public const string root = "Api";
        public const string version = "V1";
        public const string rule = $"{root}/{version}/";
        public static class AuthenticationRouting
        {
            public const string prefix = $"{rule}Authentication";
            public const string RegistrationUser = $"{prefix}/RegistrationUser";
            public const string EmailConfirmation = $"{prefix}/EmailConfirmation";
            public const string SignIn = $"{prefix}/SignIn";
            public const string SendConfirmationEmail = $"{prefix}/SendConfirmationEmail";
            public const string ValidateAccessToken = $"{prefix}/ValidateAccessToken";
            public const string GenerateRefreshToken = $"{prefix}/GenerateRefreshToken";
            public const string RevokeRefreshToken = $"{prefix}/RevokeRefreshToken";
            public const string SendForgetPasswordEmail = $"{prefix}/SendForgetPasswordEmail";
            public const string ForgetPasswordConfirmation = $"{prefix}/ForgetPasswordConfirmation";
            public const string ResetPassword = $"{prefix}/ResetPassword";
        }
        public static class MangaRouting
        {
            public const string prefix = $"{rule}Manga";
            public const string GetCategoriesHomePageMangas = $"{prefix}/GetCategoriesHomePageMangas";
            public const string GetHottestMangas = $"{prefix}/GetHottestMangas";
            public const string GetMangaByID = $"{prefix}/GetMangaByID{singleRoute}";
            public const string GetMangaByCategoryName = $"{prefix}/GetMangaByCategoryName";
            public const string GetPaginatedHottestManga = $"{prefix}/GetPaginatedHottestManga";
            public const string GetMangaByStatus = $"{prefix}/GetMangaByStatus";
            public const string MangaSearch = $"{prefix}";
        }
        public static class FavoritesRouting
        {
            public const string prefix = $"{rule}Favorites";
            public const string AddToFavorite = $"{prefix}/AddToFavorites{singleRoute}";
            public const string RemoveFromFavorite = $"{prefix}/RemoveFromFavorites{singleRoute}";
            public const string ViewFavoritesManga = $"{prefix}/ViewFavoritesManga";
        }
        public static class CompletedReadsRouting
        {
            public const string prefix = $"{rule}CompletedReads";
            public const string AddToCompletedReads = $"{prefix}/AddToCompletedReads{singleRoute}";
            public const string RemoveFromCompletedReads = $"{prefix}/RemoveFromCompletedReads{singleRoute}";
            public const string ViewCompletedReadsManga = $"{prefix}/ViewCompletedReadsManga";
        }
        public static class CurrentlyReadingRouting
        {
            public const string prefix = $"{rule}CurrentlyReading";
            public const string AddToCurrentlyReading = $"{prefix}/AddToCurrentlyReading{singleRoute}";
            public const string RemoveFromCurrentlyReading = $"{prefix}/RemoveFromCurrentlyReading{singleRoute}";
            public const string ViewCurrentlyReadingManga = $"{prefix}/ViewCurrentlyReadingManga";
        }
        public static class ReadingLaterRouting
        {
            public const string prefix = $"{rule}ReadingLater";
            public const string AddToReadingLater = $"{prefix}/AddToReadingLater{singleRoute}";
            public const string RemoveFromReadingLater = $"{prefix}/RemoveFromReadingLater{singleRoute}";
            public const string ViewReadingLaterManga = $"{prefix}/ViewReadingLaterManga";
        }
        public static class NotificationsRouting
        {
            public const string prefix = $"{rule}Notifications";
            public const string AddToNotifications = $"{prefix}/AddToNotifications{singleRoute}";
            public const string RemoveFromNotifications = $"{prefix}/RemoveFromNotifications{singleRoute}";
            public const string ViewNotificationsManga = $"{prefix}/ViewNotificationsManga";
        }
        public static class ChapterViewRouting
        {
            public const string prefix = $"{rule}ChapterView";
            public const string MarkAsRead = $"{prefix}/MarkAsRead";
            public const string MarkAsUnRead = $"{prefix}/MarkAsUnRead";
        }
        public static class ChaptersRouting
        {
            public const string prefix = $"{rule}Chapters";
            public const string ViewChaptersForSpecificMangaByLanguage = $"{prefix}/ViewChaptersForSpecificMangaByLanguage";
        }
        public static class CategoryRouting
        {
            public const string prefix = $"{rule}Categories";
            public const string GetCategories = $"{prefix}/GetCategories";
        }
        public static class UserRouting
        {
            public const string UserName = "/{username}";
            public const string prefix = $"{rule}users";
            public const string Profile = $"{prefix}/profile{UserName}";
            public const string ChangePassword = $"{prefix}/change-password";
            public const string ChangeUserName = $"{prefix}/change-username";
            public const string UploadProfileImage = $"{prefix}/upload/profile-image";
            public const string UploadCoverImage = $"{prefix}/upload/cover-image";
            public const string ChangeEmail = $"{prefix}/change-email";
            public const string ChangeEmailConfirmation = $"{prefix}/change-email/confirm";
            public const string ChangeBio = $"{prefix}/change-bio";
            public const string ChangeName = $"{prefix}/change-name";
            public const string DeleteProfileImage = $"{prefix}/profile-image";
            public const string DeleteCoverImage = $"{prefix}/cover-image";
            public const string ChangeCroppedData = $"{prefix}/crop-data";
            public const string ChangeCroppedCoverImage = $"{prefix}/cover-image/cropped-image";
        }
    }
}
