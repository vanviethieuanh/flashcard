using System;

namespace Flashcards.Class
{
    public static class Calendar
    {
        public static DateTime GetWeekend(DateTime date)
        {
            DateTime result = DateTime.Now;
            for (int i = 0; i < 7; i++)
            {
                if (date.AddDays(i).DayOfWeek == DayOfWeek.Sunday)
                {
                    result = date.AddDays(i);
                }
            }
            return result;
        }

        public static DateTime GetLastDayOfMonth(DateTime dtInput)
        {
            DateTime dtResult = dtInput;
            dtResult = dtResult.AddMonths(1);
            dtResult = dtResult.AddDays(-(dtResult.Day));
            return dtResult;
        }
        public static int SubtractDate(DateTime date)
        {
            TimeSpan ts = DateTime.Now.Subtract(date);
            int result = (int)ts.TotalDays;
            return result;
        }
    }
}
