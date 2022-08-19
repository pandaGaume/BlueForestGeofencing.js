using IOfThings.Spatial.Geography;
using System;

namespace IOfThings.Spatial.Geofencing
{
    public static class GeofencingSampleExtensions
    {
        public static DateTime GetIntersectionTime(this ISegment<IGeofencingSample> segment, ILocation loc, EllipticSystem s)
        {
            var T = s.GetDistanceBetweenTwoPoint(segment.First.Where, loc) / segment.GetLength(s);
            var dt = segment.Second.When - segment.First.When;
            return segment.First.When + dt * T;
        }
    }
}
