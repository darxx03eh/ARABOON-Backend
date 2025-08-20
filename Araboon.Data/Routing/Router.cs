namespace Araboon.Data.Routing
{
    public static class Router
    {
        public const String singleRoute = "/{id}";
        public const String root = "Api";
        public const String version = "V1";
        public const String rule = $"{root}/{version}/";
        public static class AuthenticationRouting
        {
            public const String prefix = $"{rule}Authentication";
            public const String RegistrationUser = $"{prefix}/RegistrationUser";
            public const String EmailConfirmation = $"{prefix}/EmailConfirmation";
            public const String SignIn = $"{prefix}/SignIn";
            public const String SendConfirmationEmail = $"{prefix}/SendConfirmationEmail";
            public const String ValidateAccessToken = $"{prefix}/ValidateAccessToken";
            public const String GenerateRefreshToken = $"{prefix}/GenerateRefreshToken";
            public const String RevokeRefreshToken = $"{prefix}/RevokeRefreshToken";
            public const String SendForgetPasswordEmail = $"{prefix}/SendForgetPasswordEmail";
            public const String ForgetPasswordConfirmation = $"{prefix}/ForgetPasswordConfirmation";
            public const String ResetPassword = $"{prefix}/ResetPassword";
        }
        public static class MangaRouting
        {
            public const String prefix = $"{rule}Manga";
            public const String GetCategoriesHomePageMangas = $"{prefix}/GetCategoriesHomePageMangas";
            public const String GetHottestMangas = $"{prefix}/GetHottestMangas";
            public const String GetMangaByID = $"{prefix}/GetMangaByID{singleRoute}";
            public const String GetMangaByCategoryName = $"{prefix}/GetMangaByCategoryName";
            public const String GetPaginatedHottestManga = $"{prefix}/GetPaginatedHottestManga";
            public const String GetMangaByStatus = $"{prefix}/GetMangaByStatus";
        }
        public static class FavoritesRouting
        {
            public const String prefix = $"{rule}Favorites";
            public const String AddToFavorite = $"{prefix}/AddToFavorites{singleRoute}";
            public const String RemoveFromFavorite = $"{prefix}/RemoveFromFavorites{singleRoute}";
            public const String ViewFavoritesManga = $"{prefix}/ViewFavoritesManga";
        }
        public static class CompletedReadsRouting
        {
            public const String prefix = $"{rule}CompletedReads";
            public const String AddToCompletedReads = $"{prefix}/AddToCompletedReads{singleRoute}";
            public const String RemoveFromCompletedReads = $"{prefix}/RemoveFromCompletedReads{singleRoute}";
            public const String ViewCompletedReadsManga = $"{prefix}/ViewCompletedReadsManga";
        }
        public static class CurrentlyReadingRouting
        {
            public const String prefix = $"{rule}CurrentlyReading";
            public const String AddToCurrentlyReading = $"{prefix}/AddToCurrentlyReading{singleRoute}";
            public const String RemoveFromCurrentlyReading = $"{prefix}/RemoveFromCurrentlyReading{singleRoute}";
            public const String ViewCurrentlyReadingManga = $"{prefix}/ViewCurrentlyReadingManga";
        }
        public static class ReadingLaterRouting
        {
            public const String prefix = $"{rule}ReadingLater";
            public const String AddToReadingLater = $"{prefix}/AddToReadingLater{singleRoute}";
            public const String RemoveFromReadingLater = $"{prefix}/RemoveFromReadingLater{singleRoute}";
            public const String ViewReadingLaterManga = $"{prefix}/ViewReadingLaterManga";
        }
        public static class NotificationsRouting
        {
            public const String prefix = $"{rule}Notifications";
            public const String AddToNotifications = $"{prefix}/AddToNotifications{singleRoute}";
            public const String RemoveFromNotifications = $"{prefix}/RemoveFromNotifications{singleRoute}";
            public const String ViewNotificationsManga = $"{prefix}/ViewNotificationsManga";
        }
        public static class ChapterViewRouting
        {
            public const String prefix = $"{rule}ChapterView";
            public const String MarkAsRead = $"{prefix}/MarkAsRead";
            public const String MarkAsUnRead = $"{prefix}/MarkAsUnRead";
        }
        public static class ChaptersRouting
        {
            public const String prefix = $"{rule}Chapters";
            public const String ViewChaptersForSpecificMangaByLanguage = $"{prefix}/ViewChaptersForSpecificMangaByLanguage";
        }
        public static class CategoryRouting
        {
            public const String prefix = $"{rule}Categories";
            public const String GetCategories = $"{prefix}/GetCategories";
        }
    }
}
