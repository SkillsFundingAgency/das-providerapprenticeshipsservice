using System;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime GetLastDayOfMonthDate(this DateTime date)
        {
            var lastDay = DateTime.DaysInMonth(date.Year, date.Month);

            return new DateTime(date.Year, date.Month, lastDay);
        }

        public static string ToGdsFormat(this DateTime date)
        {
            return date.ToString("d MMM yyyy");
        }

        public static string ToGdsFormatWithoutDay(this DateTime date)
        {
            return date.ToString("MMM yyyy");
        }

        public static string ToGdsFormatShortMonthWithoutDay(this DateTime date)
        {
            return date.ToString("MM yyyy");
        }

        public static string ToGdsFormatLongMonthNameWithoutDay(this DateTime date)
        {
            return date.ToString("MMMM yyyy");
        }
    }
}