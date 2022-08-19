using IOfThings.Spatial.Geography;
using IOfThings.Spatial.Text.GeoJson;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IOfThings.Spatial.Geofencing
{
    public static class GeofencingItemExtensions
    {
        public static IGeofence BindGeofence<T>( this IEnumerable<T> items, IGeofence g)
            where T : IGeofencingItem
        {
            if (items != null)
            {
                foreach (var i in items)
                {
                    i.BindGeofence<T>(g);
                }
            }
            return g;
        }
        public static void BindGeofence<T>(this T i, IGeofence g)
            where T : IGeofencingItem
        {
            i.Geofence = g;
            if (i.PreModifierIndices != null && i.PreModifierIndices.Length != 0)
            {
                foreach (var m in i.GetModifiers<IModifier>(i.PreModifierIndices))
                {
                    m.BindGeofence(g);
                }
            }
            if (i.PostModifierIndices != null && i.PostModifierIndices.Length != 0)
            {
                foreach (var m in i.GetModifiers<IModifier>(i.PostModifierIndices))
                {
                    m.BindGeofence(g);
                }
            }
            if ( i.Extensions != null && i.Extensions.Count != 0)
            {
                foreach(var e in i.Extensions)
                {
                    e.BindGeofence(i);
                }
            }
        }

        public static IEnumerable<T> GetPreModifiers<T>(this IGeofencingItem item, Func<T, bool> predicate = default) where T : IModifier => item.GetModifiers<T>(item.PreModifierIndices, predicate);
        public static IEnumerable<T> GetPostModifiers<T>(this IGeofencingItem item, Func<T, bool> predicate = default) where T : IModifier => item.GetModifiers<T>(item.PostModifierIndices, predicate);
        public static IEnumerable<T> GetModifiers<T>(this IGeofencingItem item, int[] indices, Func<T, bool> predicate = default)
        where T : IModifier
        {
            if (indices == null || indices.Length == 0 || item.Geofence == null || item.Geofence.Modifiers == null || item.Geofence.Modifiers.Count == 0)
            {
                return Enumerable.Empty<T>();
            }

            var result = new List<T>(2);

            for (int i = 0; i != indices.Length; i++)
            {
                if (i > 0 || i <= item.Geofence.Modifiers.Count)
                {
                    var m = item.Geofence.Modifiers[i];
                    if (m is T t)
                    {
                        if (predicate == default || !predicate(t))
                        {
                            // insert the item at the right place
                            if( m is IWithPriority sortable && result.Count != 0)
                            {
                                var p = sortable.Priority;
                                for(int j=0;j!= result.Count; j++)
                                {
                                    var e = result[j];
                                    if( e is IWithPriority other)
                                    {
                                        if(sortable.Priority > e.Priority)
                                        {
                                            // insert before
                                            result.Insert(j, t);
                                            break;
                                        }
                                        continue;
                                    }
                                    // insert before
                                    result.Insert(j, t);
                                    break; // all the item with any priority are stacked up
                                }
                                continue;
                            }
                            result.Add(t);
                        }
                    }
                }
            }
            return result;
        }
        public static IEnvelope ToEnvelope(this BBox bbox) => bbox == null ? null : new Envelope(bbox.SouthWest.Latitude, bbox.SouthWest.Longitude, bbox.NorthEast.Latitude, bbox.NorthEast.Longitude, bbox.SouthWest.Altitude, bbox.NorthEast.Altitude);
        public static bool IsEnabled(this IGeofencingItem item) => item.Enabled && !item.Consumed;
        public static bool IsConsumed(this IGeofencingItem item) => item.Consumed;
        public static bool IsIn(this IGeofencingItem item, DateTime when)
        {
            if(!item.IsEnabled())
            {
                return false;
            }
            if (item.PreModifierIndices != null && item.PreModifierIndices.Length != 0)
            {
                // calendar may be available if premodifiers has been defined
                var calendars = item.GetPreModifiers<ICalendar>();
                return (calendars.Count() == 0 || calendars.Any(c => c.IsIn(when)));
            }
            return true;
        }
        public static bool IsExpired(this IGeofencingItem item, DateTime when)
        {
            if (!item.IsEnabled())
            {
                return true;
            }
            if (item.PreModifierIndices != null && item.PreModifierIndices.Length != 0)
            {
                // calendar may be available if premodifiers has been defined
                var calendars = item.GetPreModifiers<ICalendar>();
                return (calendars.Count() != 0 && calendars.All(c => c.Expired(when)));
            }
            return true;
        }
        public static bool Overlap(this IGeofencingItem item, DateTime from, DateTime to) => Overlap(item, new ValueRange<DateTime>(from, to));
        public static bool Overlap(this IGeofencingItem item, IValueRange<DateTime> range)
        {
            if (item.Consumed )
            {
                return false;
            }
            var calendars = item.GetPreModifiers<ICalendar>();
            return (calendars.Count() == 0 || calendars.Any(c => c.Overlap(range)));
        }
        public static void Invalidate(this IEnumerable<IGeofencingItem> nodes, bool forward = false)
        {
            foreach (var n in nodes) n.Invalidate(forward);
        }
        public static void Validate(this IEnumerable<IGeofencingItem> nodes)
        {
            foreach (var n in nodes) n.Validate();
        }
    }
}
