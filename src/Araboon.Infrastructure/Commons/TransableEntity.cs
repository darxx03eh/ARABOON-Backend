using System.Globalization;

namespace Araboon.Infrastructure.Commons
{
    public static class TransableEntity
    {
        public static string GetTransable(string enLanguage, string arLanguage)
        {
            CultureInfo culture = Thread.CurrentThread.CurrentCulture;
            if (culture.TwoLetterISOLanguageName.ToLower().Equals("ar"))
                return arLanguage;
            return enLanguage;
        }
    }
}
