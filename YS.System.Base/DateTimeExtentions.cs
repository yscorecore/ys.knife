using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    public static class DateTimeExtentions
    {
        public static DateTime Add(this DateTime time, DateTimePart timePart, int value)
        {
            switch (timePart)
            {
                case DateTimePart.Year:
                    return time.AddYears(value);
                case DateTimePart.Month:
                    return time.AddMonths(value);
                case DateTimePart.Day:
                    return time.AddDays(value);
                case DateTimePart.Hour:
                    return time.AddHours(value);
                case DateTimePart.Minute:
                    return time.AddMinutes(value);
                case DateTimePart.Second:
                    return time.AddSeconds(value);
                case DateTimePart.Millisecond:
                    return time.AddMilliseconds(value);
                case DateTimePart.Tick:
                default:
                    return time.AddTicks(value);
            }
        }
    }
}
