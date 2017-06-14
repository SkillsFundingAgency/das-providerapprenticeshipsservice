using System;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime GetLastDayOfMonthDate(this DateTime date)
        {
            var lastDay = DateTime.DaysInMonth(date.Year, date.Month);

            return new DateTime(date.Year, date.Month, lastDay, date.Hour, date.Minute, date.Second, date.Millisecond);
        }

        public static string ToGdsFormat(this DateTime date)
        {
            return date.ToString("d MMM yyyy");
        }

        public static string ToGdsFormatWithoutDay(this DateTime date)
        {
            return date.ToString("MMM yyyy");
        }
    }
}