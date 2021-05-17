using System;

namespace IOfThings.Spatial.Geofencing
{
    public interface ICacheEntry<TKey,TValue>
    {
        TKey Key { get; }
        TValue Value { get; set; }
        TimeSpan? SlidingExpiration { get; set; }
        DateTimeOffset? AbsoluteExpiration { get; set; }
    }
}
