using System.Globalization;

namespace Araboon.Infrastructure.Commons
{
    public static class TransableEntity
    {
        public static String GetTransable(String enLanguage, String arLanguage)
        {
            CultureInfo culture = Thread.CurrentThread.CurrentCulture;
            if (culture.TwoLetterISOLanguageName.ToLower().Equals("ar"))
                return arLanguage;
            return enLanguage;
        }
    }
}
