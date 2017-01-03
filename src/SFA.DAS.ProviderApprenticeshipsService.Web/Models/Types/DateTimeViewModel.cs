using System;
using System.Globalization;
using System.Threading;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Models.Types
{

    public class DateTimeViewModel
    {
        public DateTimeViewModel()
        {
            MaxYear = System.DateTime.Now.Year + 99;
        }

        public DateTimeViewModel(DateTime? date, bool future = true)
        {
            MaxYear = future ? System.DateTime.Now.Year + 99 : System.DateTime.Now.Year;

            Day = date?.Day;
            Month = date?.Month;
            Year = date?.Year;
        }

        public DateTimeViewModel(int? day, int? month, int? year, bool future = true)
        {
            MaxYear = future ? System.DateTime.Now.Year  + 99 :  System.DateTime.Now.Year;

            Day = day;
            Month = month;
            Year = year;
        }

        public DateTimeViewModel(bool future = true)
        {
            MaxYear = future ? System.DateTime.Now.Year + 99 : System.DateTime.Now.Year;
        }

        public DateTime? DateTime => ToDateTime();

        public int? Day { get; set; }

        public int? Month { get; set; }

        private int? _year;
        public int? Year {
            get
            {
                return _year;
            }
            set
            {
                if (value < 100)
                {
                    var culture = (CultureInfo)Thread.CurrentThread.CurrentCulture.Clone();
                    culture.DateTimeFormat.Calendar.TwoDigitYearMax = MaxYear;
                    DateTime dateTimeOut;
                    _year = System.DateTime.TryParseExact(
                        $"{value.Value.ToString("00")}-1-1",
                        "yy-M-d",
                        culture,
                        DateTimeStyles.None,
                        out dateTimeOut) ? dateTimeOut.Year : value;
                }
                else _year = value;
            }
        }

        private int MaxYear { get; }

        private DateTime? ToDateTime()
        {
            return CreateDateTime(Day ?? 1, Month, Year);
        }
        
        private DateTime? CreateDateTime(int? day, int? month, int? year)
        {
            if (day.HasValue && month.HasValue && year.HasValue)
            {
                DateTime dateTimeOut;
                if (System.DateTime.TryParseExact(
                    $"{year.Value}-{month.Value}-{day.Value}",
                    "yyyy-M-d",
                    CultureInfo.CurrentCulture, DateTimeStyles.None, out dateTimeOut))
                {
                    return dateTimeOut;
                }
            }

            return null;
        }
    }
}