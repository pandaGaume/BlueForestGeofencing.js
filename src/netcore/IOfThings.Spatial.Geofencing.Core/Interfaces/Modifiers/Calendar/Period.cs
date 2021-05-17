using IOfThings.Spatial.Geofencing.Text.Json;
using System;
using System.Text.Json.Serialization;

namespace IOfThings.Spatial.Geofencing
{
    //TODO : Move Calendar logic outside to another Lib
    public class Period : ValueRange<DateTime>, IPeriod
    {
        internal static long[] RecurrenceTicks =
        {
            TimeSpan.FromDays(7).Ticks,
            TimeSpan.FromDays(1).Ticks,
            TimeSpan.FromHours(1).Ticks,
            TimeSpan.FromMinutes(1).Ticks,
            TimeSpan.FromSeconds(1).Ticks
        };

        RecurrenceType? _rt;
        int? _c;

        [JsonPropertyName("recurrence")]
        [JsonConverter(typeof(RecurrenceTypeJsonConverter))]
        public RecurrenceType? RecurrenceType { get => _rt; set => _rt = value; }

        [JsonPropertyName("count")]
        public int? RecurrenceCount { get => _c; set => _c = value; }
        public virtual bool IsValid(DateTime timestamp)
        {
            // fast path
            if(this.Contains(timestamp)) return true;
            
            if(_rt.HasValue)
            {
                long t = RecurrenceTicks[(int)_rt];
                // first verify we do not go over count occurences
                if ( _c.HasValue && _c.Value != 0 )
                {
                    long max = To.Ticks + t * _c.Value;
                    if (timestamp.Ticks > max) return false;
                }
                // then test for the period range.
                long ts = timestamp.Ticks % t;
                long from = From.Ticks % t;
                if (from > ts) return false;
                long to = To.Ticks % t;
                return to > ts;
            }
            return false;
        }
    }
}
