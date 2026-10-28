namespace MVC_Project.Models.Constants
{
    /// <summary>
    /// Indian public holidays configuration
    /// </summary>
    public static class IndianHolidays
    {
        public static readonly Dictionary<int, List<(int Month, int Day, string Name)>> Holidays = new()
        {
            {
                2024, new List<(int, int, string)>
                {
                    (1, 26, "Republic Day"),
                    (3, 8, "Maha Shivaratri"),
                    (3, 25, "Holi"),
                    (3, 29, "Good Friday"),
                    (4, 11, "Eid ul-Fitr"),
                    (4, 17, "Ram Navami"),
                    (4, 21, "Mahavir Jayanti"),
                    (5, 23, "Buddha Purnima"),
                    (6, 17, "Eid ul-Adha"),
                    (7, 17, "Muharram"),
                    (8, 15, "Independence Day"),
                    (8, 26, "Janmashtami"),
                    (9, 16, "Milad un-Nabi"),
                    (10, 2, "Gandhi Jayanti"),
                    (10, 12, "Dussehra"),
                    (10, 31, "Diwali"),
                    (11, 1, "Diwali - Day 2"),
                    (11, 15, "Guru Nanak Jayanti"),
                    (12, 25, "Christmas")
                }
            },
            {
                2025, new List<(int, int, string)>
                {
                    (1, 26, "Republic Day"),
                    (2, 28, "Maha Shivaratri"),
                    (3, 14, "Holi"),
                    (3, 30, "Good Friday"),
                    (4, 6, "Eid ul-Fitr"),
                    (4, 9, "Ram Navami"),
                    (4, 21, "Mahavir Jayanti"),
                    (5, 12, "Buddha Purnima"),
                    (6, 6, "Eid ul-Adha"),
                    (7, 6, "Muharram"),
                    (8, 15, "Independence Day"),
                    (8, 16, "Janmashtami"),
                    (9, 5, "Milad un-Nabi"),
                    (10, 2, "Gandhi Jayanti"),
                    (10, 1, "Dussehra"),
                    (10, 20, "Diwali"),
                    (10, 21, "Diwali - Day 2"),
                    (11, 5, "Guru Nanak Jayanti"),
                    (12, 25, "Christmas")
                }
            },
            {
                2026, new List<(int, int, string)>
                {
                    (1, 26, "Republic Day"),
                    (2, 17, "Maha Shivaratri"),
                    (3, 6, "Holi"),
                    (3, 29, "Good Friday"),
                    (4, 3, "Eid ul-Fitr"),
                    (3, 29, "Ram Navami"),
                    (4, 21, "Mahavir Jayanti"),
                    (5, 1, "Buddha Purnima"),
                    (5, 27, "Eid ul-Adha"),
                    (6, 26, "Muharram"),
                    (8, 15, "Independence Day"),
                    (9, 3, "Janmashtami"),
                    (8, 26, "Milad un-Nabi"),
                    (10, 2, "Gandhi Jayanti"),
                    (10, 20, "Dussehra"),
                    (11, 8, "Diwali"),
                    (11, 9, "Diwali - Day 2"),
                    (11, 25, "Guru Nanak Jayanti"),
                    (12, 25, "Christmas")
                }
            }
        };

        /// <summary>
        /// Check if a date is a holiday
        /// </summary>
        public static bool IsHoliday(DateTime date)
        {
            if (!Holidays.TryGetValue(date.Year, out var yearHolidays))
                return false;

            return yearHolidays.Any(h => h.Month == date.Month && h.Day == date.Day);
        }

        /// <summary>
        /// Get holiday name for a date
        /// </summary>
        public static string GetHolidayName(DateTime date)
        {
            if (!Holidays.TryGetValue(date.Year, out var yearHolidays))
                return null;

            var holiday = yearHolidays.FirstOrDefault(h => h.Month == date.Month && h.Day == date.Day);
            return holiday.Name;
        }

        /// <summary>
        /// Check if date is weekend (Saturday/Sunday)
        /// </summary>
        public static bool IsWeekend(DateTime date)
        {
            return date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday;
        }

        /// <summary>
        /// Check if date is a working day (not weekend, not holiday)
        /// </summary>
        public static bool IsWorkingDay(DateTime date)
        {
            return !IsWeekend(date) && !IsHoliday(date);
        }
    }
}
