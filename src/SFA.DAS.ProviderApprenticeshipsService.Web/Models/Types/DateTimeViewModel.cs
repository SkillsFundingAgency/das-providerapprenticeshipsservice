using System;
using System.Globalization;
using System.Threading;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Models.Types
{

    public class DateTimeViewModel
    {
        public DateTimeViewModel()
        {
            MaxYear = System.DateTime.Now.Year + 100;
        }

        public DateTimeViewModel(DateTime? date, bool future = true)
        {
            Day = date?.Day;
            Month = date?.Month;
            Year = date?.Year;
            MaxYear = future ? System.DateTime.Now.Year + 100 : System.DateTime.Now.Year;
        }

        public DateTimeViewModel(int? day, int? month, int? year, bool future = true)
        {
            Day = day;
            Month = month;
            Year = year;
            MaxYear = future ? System.DateTime.Now.Year  + 100 :  System.DateTime.Now.Year;
        }

        public DateTimeViewModel(bool future = true)
        {
            MaxYear = future ? System.DateTime.Now.Year + 100 : System.DateTime.Now.Year;
        }

        public DateTime? DateTime => ToDateTime();

        public int? Day { get; set; }

        public int? Month { get; set; }

        public int? Year { get; set; }

        private int MaxYear { get; }

        private DateTime? ToDateTime()
        {
            return CreateDateTime(Day ?? 1, Month, Year);
        }
        
        private DateTime? CreateDateTime(int? day, int? month, int? year)
        {
            if (day.HasValue && month.HasValue && year.HasValue)
            {
                var culture = (CultureInfo)Thread.CurrentThread.CurrentCulture.Clone();
                culture.DateTimeFormat.Calendar.TwoDigitYearMax = MaxYear;
                var yFormat = Year < 99 ? "yy" : "yyyy" ;
                DateTime dateOfBirthOut;
                if (System.DateTime.TryParseExact(
                    $"{year.Value}-{month.Value}-{day.Value}",
                    $"{yFormat}-M-d",
                    culture, DateTimeStyles.None, out dateOfBirthOut))
                {
                    return dateOfBirthOut;
                }
            }

            return null;
        }
    }
}