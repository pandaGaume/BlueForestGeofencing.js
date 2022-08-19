using IOfThings.Spatial.Geography;
using System.Collections.Generic;
using System.Numerics;

namespace IOfThings.Spatial.Geofencing
{
    public static class GCircleExtensions
    {
        public static ILocation Center(this IGCircle shape, Matrix4x4 transform) => shape.GetLocation(transform);
        public static IEnumerable<ILocation> Polyline(this IGCircle shape, Matrix4x4 transform, int n = 32, bool close = true, EllipticSystem system = null)
        {
            var center = shape.Center(transform);
            return center.ToCirclePolyline(shape.Radius.SemiMajorAxis, n, close, system);
        }
        public static RelativePosition GetDistanceStatus(this IGCircle shape, double D) => D < shape.Radius.SemiMajorAxis ? RelativePosition.Inside : D > shape.Radius.SemiMajorAxis ? RelativePosition.Outside : RelativePosition.OnEdge;

        public static RelativePosition GetRelativePosition(this IGCircle shape, ILocation l)
        {
            return RelativePosition.Unknown;
        }
    }
}
