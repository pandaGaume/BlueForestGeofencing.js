
using IOfThings.Spatial.Geography;
using IOfThings.Spatial.Text.GeoJson;
using System.Collections.Generic;

namespace IOfThings.Spatial.Geofencing
{
    public static class GeofencingItemExtensions
    {
        internal static IGeofence BindGeofence<T>( this IEnumerable<T> items, IGeofence g)
            where T : IGeofencingItem
        {
            if( items != null)
            {
                foreach (var i in items) i.Geofence = g;
            }
            return g;
        }

        public static IEnumerable<IModifier> GetPreModifier(IGeofencingItem item) => item.GetModifiers(item.PreModifierIndices);
        public static IEnumerable<IModifier> GetPostModifier(IGeofencingItem item) => item.GetModifiers(item.PostModifierIndices);
        public static IEnumerable<IModifier> GetModifiers(this IGeofencingItem item, int[] indices)
        {
            if (indices == null || item.Geofence == null || item.Geofence.Modifiers == null) yield break;
            for (int i = 0; i != indices.Length; i++)
            {
                if (i > 0 || i <= item.Geofence.Modifiers.Count)
                {
                    yield return item.Geofence.Modifiers[i];
                }
            }
        }

        public static IEnvelope ToEnvelope(this BBox bbox) => bbox == null ? Envelope.Empty() : new Envelope(bbox.SouthWest.Latitude, bbox.SouthWest.Longitude, bbox.NorthEast.Latitude, bbox.NorthEast.Longitude, bbox.SouthWest.Altitude, bbox.NorthEast.Altitude);

        public static bool IsEnabled(this ITreeNode node) => node.Enabled;
        public static bool IsConsumed(this ITreeNode node) => node.Consumed;

    }
}
