using IOfThings.Spatial.Geography;
using IOfThings.Spatial.Text.GeoJson;
using System;
using System.Linq;

namespace IOfThings.Spatial.Geofencing
{
    public static class  ShapeExtensions
    {
        public static bool IsInside(this RelativePosition p) => p == RelativePosition.Inside || p == RelativePosition.OnEdge;
        public static RelativePosition GetDistanceStatus(this IShape shape, double D) => D < shape.Radius.SemiMajorAxis ? RelativePosition.Outside : D > shape.Radius.SemiMajorAxis ? RelativePosition.Inside : RelativePosition.OnEdge;

        public static IEnvelope BuildEnvelope(this IShape shape)
        {
            if (shape.Geofence == null || shape.Geofence.Geometry == null ) return Envelope.Empty();
            var geometry = shape.Geofence.Geometry;
            return (geometry.BBox ?? geometry.BuildBBox()).ToEnvelope();
        }

        public static IEnvelope BuildEnvelopeWithRadius(this IShape shape)
        {
            if (shape.Geofence == null || shape.Geofence.Geometry == null) return Envelope.Empty();

            var geometry = shape.Geofence.Geometry;

            if (!Radius.IsNullOrEmpty(shape.Radius))
            {
                EllipticSystem s = shape.Geofence.GeodeticSystem?? EllipticSystem.WGS84;

                Position c = null;
                double? min = null;
                double? max = null;

                if (geometry is GeoJsonPoint p)
                {
                    c = p.Position;
                    min = c.Altitude;
                    max = c.Altitude;
                }
                else
                {
                    var env = geometry.BBox ?? geometry.BuildBBox();
                    c = env.GetCenter();
                    min = env.SouthWest.Altitude;
                    max = env.NorthEast.Altitude;
                }

                var lat = c.Latitude;
                var lon = c.Longitude;
                var a = shape.Radius.SemiMajorAxis;
                double b = shape.Radius.HasSemiMinorAxis ? shape.Radius.SemiMinorAxis.Value : a;

                s.GetLocationAtDistanceAzimuth(lat, lon, b, 0, out double north, out double dummy);
                s.GetLocationAtDistanceAzimuth(lat, lon, a, 90, out dummy, out double east);
                s.GetLocationAtDistanceAzimuth(lat, lon, b, 180, out double south, out dummy);
                s.GetLocationAtDistanceAzimuth(lat, lon, a, 270, out dummy, out double west);
                if (shape.Elevation != null)
                {
                    var E = shape.Elevation;
                    min = min.HasValue ? Math.Min(min.Value, E.HasFrom ? E.From : min.Value) :
                                                             E.HasFrom ? (double?)E.From : null;
                    max = max.HasValue ? Math.Max(max.Value, E.HasTo ? E.To : max.Value) :
                                                             E.HasTo ? (double?)E.To : null;
                }
                return new Envelope(south, west, north, east, min, max);
            }
            return (geometry.BBox ?? geometry.BuildBBox())?.ToEnvelope();
        }

        public static IEnvelope BuildEnvelopeWithRadius(this IPolyline shape)
        {
            if (shape.Geofence == null || shape.Geofence.Geometry == null) return Envelope.Empty();
            var geometry = shape.Geofence.Geometry;
            if (!Radius.IsNullOrEmpty(shape.Radius))
            {
                EllipticSystem s = shape.Geofence.GeodeticSystem ?? EllipticSystem.WGS84;

                // doing all the calculation as radian save a lot of ressources.
                ILocation[] locAsRadian = null;
                if (geometry is GeoJsonMultiPoint mp)
                {
                    locAsRadian = mp.Positions.Select(p=>new Location(p.Latitude* Ellipsoid.d2r, p.Longitude * Ellipsoid.d2r, p.Altitude)).ToArray();
                }
                else if (geometry is GeoJsonLineString ls)
                {
                    locAsRadian = ls.Positions.Select(p => new Location(p.Latitude * Ellipsoid.d2r, p.Longitude * Ellipsoid.d2r, p.Altitude)).ToArray();
                }

                if (locAsRadian != null)
                {
                    ILocation a = null, b = null;
                    double lat, lon;
                    var envAsRadian = new Envelope();
                    for (int i = 0; i < locAsRadian.Length; i++)
                    {
                        b = locAsRadian[i];
                        if (a != null)
                        {
                            var bearing = s.GetBearing(a.Latitude, a.Longitude, b.Latitude, b.Longitude, false);
                            var left = bearing - Math.PI / 2;
                            var right = bearing + Math.PI / 2;

                            s.GetLocationAtDistanceAzimuth(a.Latitude, a.Longitude, left, 0, out lat, out lon, false);
                            envAsRadian.AddInPlace(lat, lon);
                            s.GetLocationAtDistanceAzimuth(a.Latitude, a.Longitude, left, 0, out lat, out lon, false);
                            envAsRadian.AddInPlace(lat, lon);
                            s.GetLocationAtDistanceAzimuth(a.Latitude, a.Longitude, right, 0, out lat, out lon, false);
                            envAsRadian.AddInPlace(lat, lon);
                            s.GetLocationAtDistanceAzimuth(a.Latitude, a.Longitude, right, 0, out lat, out lon, false);
                            envAsRadian.AddInPlace(lat, lon);
                        }
                        a = b;
                    }
                    return envAsRadian.ToDegree();
                }
            }
            return (geometry.BBox ?? geometry.BuildBBox())?.ToEnvelope();
        }

        public static ENUSystem GetENU(this IShape shape, ILocation center)
        {
            var s = shape.Geofence?.GeodeticSystem;
            if (s != null && s is ENUSystem enu) return enu;
            return new ENUSystem(center, s?.Ellipsoid ?? Ellipsoid.WGS84);
        }
    }
}
