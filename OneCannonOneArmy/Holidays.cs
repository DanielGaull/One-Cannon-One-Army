using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace OneCannonOneArmy
{
    public static class Holidays
    {
        const int HALLOWEEN_MONTH = 10;
        const int HALLOWEEN_DAY = 31;
        const int CHRISTMAS_MONTH = 12;
        const int CHRISTMAS_DAY = 25;
        const int THANKSGIVING_MONTH = 11;
        const DayOfWeek THANKSGIVING_DAY_OF_WEEK = DayOfWeek.Thursday;
        const int DAY_OF_THANKSGIVING = 4;
        const int INDEP_MONTH = 7;
        const int INDEP_DAY = 4;
        const int SUPER_BOWL_MONTH = 2;
        const DayOfWeek SUPER_BOWL_DAY_OF_WEEK = DayOfWeek.Sunday;
        const int DAY_OF_SUPER_BOWL = 1;

        public const int DAYS_IN_WEEK = 7;

        public static DateTime Halloween(int year)
        {
            return new DateTime(year, HALLOWEEN_MONTH, HALLOWEEN_DAY);
        }
        public static DateTime Christmas(int year)
        {
            return new DateTime(year, CHRISTMAS_MONTH, CHRISTMAS_DAY);
        }
        public static DateTime Easter(int year)
        {
            // Code found at 
            // stackoverflow.com/questions/2510383/how-can-i-calculate-what-date-good-friday-falls-on-given-a-year
            int day = 0;
            int month = 0;

            int g = year % 19;
            int c = year / 100;
            int h = (c - c / 4 - (8 * c + 13) / 25 + 19 * g + 15) % 30;
            int i = h - h / 28 * (1 - h / 28 * (29 / (h + 1)) * ((21 - g) / 11));

            day = i - ((year + year / 4 + i + 2 - c + c / 4) % 7) + 28;
            month = 3;

            if (day > 31)
            {
                month++;
                day -= 31;
            }

            return new DateTime(year, month, day);
        }
        public static DateTime Thanksgiving(int year)
        {
            DateTime returnTime = new DateTime(year, THANKSGIVING_MONTH, 1);

            // Thanksgiving is on the 4th Thursday of November
            // We first want to get the day to a Thursday...
            while (returnTime.DayOfWeek != THANKSGIVING_DAY_OF_WEEK)
            {
                returnTime = returnTime.AddDays(1.0);
            }
            // ...now we can simply add 3 weeks to get the 4th Thursday
            returnTime = returnTime.AddDays(DAYS_IN_WEEK * (DAY_OF_THANKSGIVING - 1));

            return returnTime;
        }
        public static DateTime SuperBowl(int year)
        {
            DateTime returnTime = new DateTime(year, SUPER_BOWL_MONTH, 1);

            while (returnTime.DayOfWeek != SUPER_BOWL_DAY_OF_WEEK)
            {
                returnTime = returnTime.AddDays(1.0);
            }
            returnTime = returnTime.AddDays(DAYS_IN_WEEK * (DAY_OF_SUPER_BOWL - 1));

            return returnTime;
        }
        public static DateTime BlackFriday(int year)
        {
            return Holidays.Thanksgiving(year).AddDays(1.0);
        }
        public static DateTime IndependenceDay(int year)
        {
            return new DateTime(year, INDEP_MONTH, INDEP_DAY);
        }

        public static bool IsHalloweenWeek()
        {
            DateTime now = DateTime.Now;
            DateTime halloween = Halloween(now.Year);
            DateTimeFormatInfo dfi = DateTimeFormatInfo.CurrentInfo;
            Calendar c = dfi.Calendar;
            int halloweenWeek = c.GetWeekOfYear(halloween, dfi.CalendarWeekRule, dfi.FirstDayOfWeek);
            int thisWeek = c.GetWeekOfYear(now, dfi.CalendarWeekRule, dfi.FirstDayOfWeek);
            return halloweenWeek == thisWeek;
        }
        public static bool IsChristmasWeek()
        {
            DateTime now = DateTime.Now;
            DateTime christmas = Christmas(now.Year);
            DateTimeFormatInfo dfi = DateTimeFormatInfo.CurrentInfo;
            Calendar c = dfi.Calendar;
            int christmasWeek = c.GetWeekOfYear(christmas, dfi.CalendarWeekRule, dfi.FirstDayOfWeek);
            int thisWeek = c.GetWeekOfYear(now, dfi.CalendarWeekRule, dfi.FirstDayOfWeek);
            return christmasWeek == thisWeek;
        }
        public static bool IsEasterWeek()
        {
            DateTime now = DateTime.Now;
            DateTime easter = Easter(now.Year);
            DateTimeFormatInfo dfi = DateTimeFormatInfo.CurrentInfo;
            Calendar c = dfi.Calendar;
            int easterWeek = c.GetWeekOfYear(easter, dfi.CalendarWeekRule, dfi.FirstDayOfWeek);
            int thisWeek = c.GetWeekOfYear(now, dfi.CalendarWeekRule, dfi.FirstDayOfWeek);
            return easterWeek == thisWeek;
        }
        public static bool IsThanksgivingWeek()
        {
            DateTime now = DateTime.Now;
            DateTime thanksgiving = Thanksgiving(now.Year);
            DateTimeFormatInfo dfi = DateTimeFormatInfo.CurrentInfo;
            Calendar c = dfi.Calendar;
            int thanksgivingWeek = c.GetWeekOfYear(thanksgiving, dfi.CalendarWeekRule, dfi.FirstDayOfWeek);
            int thisWeek = c.GetWeekOfYear(now, dfi.CalendarWeekRule, dfi.FirstDayOfWeek);
            return thanksgivingWeek == thisWeek;
        }
        public static bool IsBlackFriday()
        {
            DateTime now = DateTime.Now;
            DateTime blackFriday = BlackFriday(now.Year);
            return (blackFriday.Day == now.Day) && (blackFriday.Month == now.Month);
        }
        public static bool IsIndependenceDayWeek()
        {
            DateTime now = DateTime.Now;
            DateTime indepDay = IndependenceDay(now.Year);
            DateTimeFormatInfo dfi = DateTimeFormatInfo.CurrentInfo;
            Calendar c = dfi.Calendar;
            int indepWeek = c.GetWeekOfYear(indepDay, dfi.CalendarWeekRule, dfi.FirstDayOfWeek);
            int thisWeek = c.GetWeekOfYear(now, dfi.CalendarWeekRule, dfi.FirstDayOfWeek);
            return indepWeek == thisWeek;
        }
        public static bool IsSuperBowlWeek()
        {
            DateTime now = DateTime.Now;
            DateTime superBowlDay = SuperBowl(now.Year);
            DateTimeFormatInfo dfi = DateTimeFormatInfo.CurrentInfo;
            Calendar c = dfi.Calendar;
            int superBowlWk = c.GetWeekOfYear(superBowlDay, dfi.CalendarWeekRule, dfi.FirstDayOfWeek);
            int thisWeek = c.GetWeekOfYear(now, dfi.CalendarWeekRule, dfi.FirstDayOfWeek);
            return superBowlWk == thisWeek;
        }
        public static bool IsWinter()
        {
            int month = DateTime.Now.Month;
            if (month == 12)
            {
                return true;
            }
            return false;
        }
    }
}
