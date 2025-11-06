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
            public const string LogOut = $"{prefix}/LogOut";
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
            public const string GetMangaComments = $"{prefix}{singleRoute}/comments";
            public const string GetMangaCommentsCounts = $"{prefix}{singleRoute}/comments-count";
            public const string AddNewManga = $"{prefix}";
            public const string DeleteManga = $"{prefix}{singleRoute}";
            public const string DeleteMangaImage = $"{prefix}{singleRoute}/image";
            public const string UploadNewImage = $"{prefix}/upload-image";
            public const string ArabicAvailableOrUnAvailable = $"{prefix}{singleRoute}/arabic-toggle";
            public const string EnglishAvailableOrUnAvailableAsync = $"{prefix}{singleRoute}/english-toggle";
            public const string ActivateOrDeActivateManga = $"{prefix}{singleRoute}/active-toggle";
            public const string UpdateMangaInfo = prefix;
            public const string GetMangaForDashboard = $"{prefix}/dashboard";
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
            public const string ViewChapterImages = $"{prefix}/images";
            public const string ChapterRead = $"{prefix}/read";
            public const string AddNewChapter = prefix;
            public const string DeleteExistingChapter = $"{prefix}{singleRoute}";
            public const string UpdateExistingChapter = prefix;
            public const string UploadChapterImage = $"{prefix}/upload-image";
            public const string UploadChapterImages = $"{prefix}/upload-images";
        }
        public static class CategoryRouting
        {
            public const string prefix = $"{rule}Categories";
            public const string GetCategories = $"{prefix}/GetCategories";
            public const string AddNewCategory = prefix;
            public const string DeleteCategory = $"{prefix}{singleRoute}";
            public const string ActivateCategory = $"{prefix}{singleRoute}/active";
            public const string DeActivateCategory = $"{prefix}{singleRoute}/deactive";
            public const string UpdateCategory = prefix;
            public const string GetDashboardCategories = prefix;
            public const string GetCategoryById = $"{prefix}{singleRoute}";
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
        public static class CommentRouting
        {
            public const string prefix = $"{rule}comments";
            public const string AddComment = $"{prefix}";
            public const string DeleteComment = $"{prefix}{singleRoute}";
            public const string UpdateComment = $"{prefix}{singleRoute}";
            public const string AddLike = $"{prefix}{singleRoute}/like";
            public const string DeleteLike = $"{prefix}{singleRoute}/like";
            public const string GetCommentReplies = $"{prefix}{singleRoute}/replies";
        }
        public static class ReplyRouting
        {
            public const string prefix = $"{rule}replies";
            public const string AddReply = $"{prefix}";
            public const string DeleteReply = $"{prefix}{singleRoute}";
            public const string UpdateReply = $"{prefix}{singleRoute}";
            public const string AddLike = $"{prefix}{singleRoute}/like";
            public const string DeleteLike = $"{prefix}{singleRoute}/like";
        }
        public static class RatingRouting
        {
            public const string prefix = $"{rule}ratings";
            public const string DeleteRate = $"{prefix}{singleRoute}";
            public const string GetRate = $"{prefix}/manga{singleRoute}";
        }
    }
}
