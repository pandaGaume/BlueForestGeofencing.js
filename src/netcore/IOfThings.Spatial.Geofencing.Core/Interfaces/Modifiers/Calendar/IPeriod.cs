using System;

namespace IOfThings.Spatial.Geofencing
{
    public enum RecurrenceType
    {
        week,
        day,
        hour,
        min,
        seconds
    }

    public interface IPeriod : IValueRange<DateTime>
    {
        /// <summary>
        /// the type xxx of reccurence. The meaning is "occur once a xxxx"
        /// </summary>
        public RecurrenceType? RecurrenceType { get; set; }
        public int? RecurrenceCount { get; set; }
        public bool IsValid(DateTime timestamp);
        public bool IsValid(IValueRange<DateTime> range);
    }
}
