using System;
using System.Globalization;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Models.Types
{
    public class DateTimeViewModel
    {
        public DateTimeViewModel()
        {
        }

        public DateTimeViewModel(DateTime? date)
        {
            if (date != null)
            {
                Day = date.Value.Day;
                Month = date.Value.Month;
                Year = date.Value.Year;
            }
        }

        public DateTimeViewModel(int? day, int? month, int? year)
        {
            Day = day;
            Month = month;
            Year = year;
        }

        public int? Day { get; }

        public int? Month { get; }

        public int? Year { get; }

        public DateTime? ToDateTime()
        {
            var year = Year < 1000 && Year > 0 
                ? Year + 2000 
                : Year;
            return GetDateTime(Day ?? 1, Month, year);
        }
        
        private DateTime? GetDateTime(int? day, int? month, int? year)
        {
            if (day.HasValue && month.HasValue && year.HasValue)
            {
                DateTime dateOfBirthOut;
                if (DateTime.TryParseExact(
                    $"{year.Value}-{month.Value}-{day.Value}",
                    "yyyy-M-d",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out dateOfBirthOut))
                {
                    return dateOfBirthOut;
                }
            }

            return null;
        }
    }
}