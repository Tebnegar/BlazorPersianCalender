namespace BlazorApp.Services.Calendars;

using System.Globalization;

public class PersianCalendarService
{
    private readonly PersianCalendar _persianCalendar = new();

    public class DayInfo
    {
        public int DayNumber { get; set; }
        public bool IsCurrentDay { get; set; }
        public bool IsOtherMonth { get; set; }
        public int TaskCount { get; set; }
    }

    public readonly string[] MonthNames =
    [
        "فروردین", "اردیبهشت", "خرداد", "تیر", "مرداد", "شهریور",
        "مهر", "آبان", "آذر", "دی", "بهمن", "اسفند"
    ];

    private DateTime Today => DateTime.Now;

    public int GetCurrentYear() => _persianCalendar.GetYear(Today);
    public int GetCurrentMonth() => _persianCalendar.GetMonth(Today);
    private int GetCurrentDay() => _persianCalendar.GetDayOfMonth(Today);

    public List<DayInfo> GetMonthDays(int year, int month)
    {
        var days = new List<DayInfo>();
        var daysInMonth = _persianCalendar.GetDaysInMonth(year, month);
        var firstDayOfMonth = _persianCalendar.ToDateTime(year, month, 1, 0, 0, 0, 0);
        var startDayOfWeek = ((int)firstDayOfMonth.DayOfWeek + 1) % 7;

        // Previous month days
        if (startDayOfWeek > 0)
        {
            var previousMonth = month == 1 ? 12 : month - 1;
            var previousYear = month == 1 ? year - 1 : year;
            var daysInPreviousMonth = _persianCalendar.GetDaysInMonth(previousYear, previousMonth);
            
            for (var i = daysInPreviousMonth - startDayOfWeek + 1; i <= daysInPreviousMonth; i++)
            {
                days.Add(new DayInfo 
                { 
                    DayNumber = i, 
                    IsOtherMonth = true,
                    TaskCount = 0
                });
            }
        }

        // Current month days
        for (var day = 1; day <= daysInMonth; day++)
        {
            days.Add(new DayInfo
            {
                DayNumber = day,
                IsCurrentDay = day == GetCurrentDay() && 
                              month == GetCurrentMonth() &&
                              year == GetCurrentYear(),
                IsOtherMonth = false,
                TaskCount = Random.Shared.Next(0, 4)
            });
        }

        // Next month days
        var remainingDays = 42 - days.Count;
        for (var day = 1; day <= remainingDays; day++)
        {
            days.Add(new DayInfo 
            { 
                DayNumber = day, 
                IsOtherMonth = true,
                TaskCount = 0
            });
        }

        return days;
    }

    public List<(int Year, int Month)> GetNextTwelveMonths(int currentYear, int currentMonth)
    {
        var months = new List<(int Year, int Month)>();
        
        for (var i = 0; i < 12; i++)
        {
            months.Add((currentYear, currentMonth));
            
            if (currentMonth == 12)
            {
                currentMonth = 1;
                currentYear++;
            }
            else
            {
                currentMonth++;
            }
        }
        
        return months;
    }

    public static string ConvertToFarsiNumbers(string input)
    {
        string[] farsiNumbers = ["۰", "۱", "۲", "۳", "۴", "۵", "۶", "۷", "۸", "۹"];
        return input.Select(c => char.IsDigit(c) ? farsiNumbers[c - '0'] : c.ToString())
                   .Aggregate((a, b) => a + b);
    }
}