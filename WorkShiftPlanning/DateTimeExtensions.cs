namespace WorkShiftPlanning
{
    public static class DateTimeExtensions
    {
        public static DateTime GetStartOfWeek(this DateTime date)
        {
            DayOfWeek firstDayOfWeek = DayOfWeek.Monday;
            int diff = (7 + (date.DayOfWeek - firstDayOfWeek)) % 7;
            return date.AddDays(-1 * diff).Date;
        }

        public static DateTime GetLastMondayOfMonth(this DateTime date)
        {
            var lastDay = new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month));
            while (lastDay.DayOfWeek != DayOfWeek.Monday)
                lastDay = lastDay.AddDays(-1);
            return lastDay;
        }

        public static DateTime GetFirstMondayOfMonth(this DateTime date)
        {
            var firstDay = new DateTime(date.Year, date.Month, 1);
            while (firstDay.DayOfWeek != DayOfWeek.Monday)
                firstDay = firstDay.AddDays(1);
            return firstDay;
        }

        public static DateTime GetNthDayOfWeek(this DateTime date, DayOfWeek dayOfWeek, int occurrence)
        {
            var firstDay = new DateTime(date.Year, date.Month, 1);
            int count = 0;

            for (int day = 1; day <= DateTime.DaysInMonth(date.Year, date.Month); day++)
            {
                var tmpDate = new DateTime(date.Year, date.Month, day);
                if (tmpDate.DayOfWeek == dayOfWeek)
                {
                    count++;
                    if (count == occurrence)
                        return date;
                }
            }

            return firstDay;
        }

        public static List<DateTime> GetHolidays(int year)
        {
            // Customizable list of holidays
            var holidays = new List<DateTime>
            {
                new DateTime(year, 1, 1),
                new DateTime(year, 1, 2),
                new DateTime(year, 3, 3),
                new DateTime(year, 4, 10),
                new DateTime(year, 4, 13),
                new DateTime(year, 5, 1),
                new DateTime(year, 5, 6),
                new DateTime(year, 5, 24),
                new DateTime(year, 9, 6),
                new DateTime(year, 9, 22),
                new DateTime(year, 12, 24),
                new DateTime(year, 12, 25),
            };

            // Example for first Monday of a pecific monthand year
            //holidays.Add((new DateTime(year, 9, 1)).GetFirstMondayOfMonth());
            // Examples for last Monday of a pecific monthand year
            //holidays.Add((new DateTime(year, 5, 1)).GetLastMondayOfMonth());
            // Example for a 4th Thursday of a month and year
            //holidays.Add((new DateTime(year, 11, 1).GetNthDayOfWeek(DayOfWeek.Thursday, 4)));

            return holidays;
        }
    }
}
