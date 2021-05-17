using IOfThings.Spatial.Geofencing;
using System;
using System.Linq;

namespace IOfThings.Spatial.Geofencing
{
    public static class CalendarExtensions
    {
        public static bool IsValid(this ICalendar calendar, DateTime timestamp)
        {
            return !calendar.Expired(timestamp) && calendar.Validities == null || calendar.Validities.Any(v => v.IsValid(timestamp));
        }

        public static bool Expired(this ICalendar calendar, DateTime timestamp) => calendar.AbsoluteExpiration != null && calendar.AbsoluteExpiration < timestamp;
        public static bool Expired(this ICalendar calendar) => calendar.AbsoluteExpiration != null && calendar.AbsoluteExpiration < DateTime.UtcNow;
    }
}
