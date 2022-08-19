using IOfThings.Spatial.Geofencing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IOfThings.Spatial.Geofencing
{
    public static class CalendarExtensions
    {
        public static RecurrenceType? Parse(this string recurrenceStr)
        {
            if(recurrenceStr != null)
            {
                if(Enum.TryParse(typeof(RecurrenceType), recurrenceStr, out object res))
                {
                    return (RecurrenceType)res;
                }
            }
            return null;
        }
        public static bool IsIn(this ICalendar calendar, DateTime timestamp)
        {
            if (calendar.Expired(timestamp))
            {
                return false;
            }
            if (calendar.Periods == null || calendar.Periods.Count == 0)
            {
                return true;
            }
            return calendar.Periods.Any(period => period.IsValid(timestamp));
        }
        public static bool Overlap(this ICalendar calendar, DateTime from, DateTime to)
        {
            if (calendar.Expired(from))
            {
                return false;
            }
            if (calendar.Periods == null || calendar.Periods.Count == 0)
            {
                return true;
            }
            var range = new ValueRange<DateTime>(from, to);
            return calendar.Periods.Any(period => period.Intersect(range));
        }
        public static bool Overlap(this ICalendar calendar, IValueRange<DateTime> range)
        {
            if (calendar.Expired(range.From))
            {
                return false;
            }
            if (calendar.Periods == null || calendar.Periods.Count == 0)
            {
                return true;
            }

            return calendar.Periods.Any(period => period.Intersect(range));
        }
        public static bool Intersect(this IPeriod p, IValueRange<DateTime> range) => p.IsValid(range);
        public static bool Expired(this ICalendar calendar, DateTime timestamp) => calendar.AbsoluteExpiration != null && calendar.AbsoluteExpiration < timestamp;
        public static bool Expired(this ICalendar calendar) => calendar.AbsoluteExpiration != null && calendar.AbsoluteExpiration < DateTime.UtcNow;
        public static Period Trim(this Period p, IValueRange<DateTime> range)
        {
            return range.Intersect(p)? new Period(p.From < range.From ? range.From : p.From, p.To > range.To ? range.To : p.To) : null;
        }
        public static IEnumerable<Period> Trim(this IEnumerable<Period> periods, IValueRange<DateTime> range)
        {
            foreach (var p in periods)
            {
                var trimmed = range != null ? p.Trim(range) : new Period(p);
                if (trimmed != null)
                {
                    yield return trimmed;
                }
            }
        }
    }
}
