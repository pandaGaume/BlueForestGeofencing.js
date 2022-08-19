using IOfThings.Spatial.Geography;
using IOfThings.Spatial.Text.GeoJson;
using System;
using System.Linq;
using System.Numerics;

namespace IOfThings.Spatial.Geofencing
{
    public static class ShapeExtensions
    {
        public static bool IsInside(this RelativePosition p) => p == RelativePosition.Inside || p == RelativePosition.OnEdge;
        public static RelativePosition GetRelativePosition(this IGeofencingShape shape, ILocation l)
        {
            if( shape is IGCircle c)
            {
                return GetRelativePosition(c,l);
            }
            if (shape is IGPolygon p)
            {
                return GetRelativePosition(p, l);
            }
            if (shape is IGRectangle r)
            {
                return GetRelativePosition(r, l);
            }
            return RelativePosition.Unknown;
        }
        public static IEnvelope BuildLocalEnvelope(this IGeofencingShape shape)
        {
            if (shape.Geofence == null || shape.Geofence.Geometry == null) return null;
            var geometry = GetGeometry(shape);
            return (geometry.BBox ?? geometry.BuildBBox()).ToEnvelope();
        }
        public static IEnvelope BuildLocalEnvelopeWithRadius(this IGeofencingShape shape, int centerIndex = 0)
        {
            if (shape.Geofence == null || shape.Geofence.Geometry == null) return null;

            var geometry = shape.Geofence.Geometry;

            if (!Radius.IsNullOrEmpty(shape.Radius))
            {
                EllipticSystem s = shape.Geofence.GeodeticSystem ?? EllipticSystem.WGS84;

                Position c = null;
                double? min = null;
                double? max = null;

                if (geometry is GeoJsonFeature f)
                {
                    geometry = f.Geometry;
                }

                if (geometry is GeoJsonGeometryCollection gc)
                {
                    geometry = gc.Geometries.ElementAt(shape.GeometryIndex);
                }

                if (geometry is GeoJsonPoint p)
                {
                    c = p.Position;
                }

                if (geometry is GeoJsonMultiPoint mp)
                {
                    c = mp.Positions[centerIndex];
                }
                else
                {
                    var env = geometry.BBox ?? geometry.BuildBBox();
                    c = env.GetCenter();
                }

                var lat = Location.NormalizeLatitude(c.Latitude);
                var lon = Location.NormalizeLongitude(c.Longitude);
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
        public static ENUSystem GetENU(this IGeofencingShape shape, ILocation center)
        {
            var s = shape.Geofence?.GeodeticSystem;
            if (s != null && s is ENUSystem enu) return enu;
            return new ENUSystem(center, s?.Ellipsoid ?? Ellipsoid.WGS84);
        }
        public static IGeoJsonObject GetGeometry(this IGeofencingShape shape)
        {
            if (shape?.Geofence?.Geometry != null)
            {
                var geometry = shape.Geofence.Geometry;

                // accept feature as indirection
                if (geometry is GeoJsonFeature f)
                {
                    geometry = f.Geometry;
                }

                if (geometry is GeoJsonGeometryCollection gc)
                {
                    geometry = gc.Geometries.ElementAt(shape.GeometryIndex);
                }

                return geometry;
            }
            return null;
        }
        public static ILocation GetLocation(this IGeofencingShape shape, Matrix4x4 transform)
        {
            var geometry = GetGeometry(shape);
            if (geometry != null)
            {
                if (geometry is GeoJsonPoint p)
                {
                    var c = p.Coordinates;
                    // Note : Location already normalize coordinate
                    return new Location(c[1], c[0]).TransformInPlace(transform);
                }

                if (geometry is GeoJsonMultiPoint mp)
                {
                    var c = mp.Coordinates[shape.GeometryIndex];
                    // Note : Location already normalize coordinate
                    return new Location(c[1], c[0]).TransformInPlace(transform);
                }
            }
            return default(ILocation);
        }
        public static GeoPath GetGeoPath(this IGeofencingShape shape, Matrix4x4 transform)
        {
            if (shape?.Geofence?.Geometry != null)
            {
                var geometry = shape.Geofence.Geometry;

                // accept feature as indirection
                if (geometry is GeoJsonFeature f)
                {
                    geometry = f.Geometry;
                }
                // get the geometry at index if it's collection
                if (geometry is GeoJsonGeometryCollection gc)
                {
                    geometry = gc.Geometries.ElementAt(shape.GeometryIndex);
                }
                if (geometry is GeoJsonMultiPoint mp)
                {
                    var segment = new TrackSegment(mp.Coordinates.Select(c => new Point(Location.FromCoordinates(c).TransformInPlace(transform))));
                    return new GeoPath(new Track(segment));
                }
                if (geometry is GeoJsonLineString ls)
                {
                    var segment = new TrackSegment(ls.Coordinates.Select(c => new Point(Location.FromCoordinates(c).TransformInPlace(transform))));
                    return new GeoPath(new Track(segment));
                }
                if (geometry is GeoJsonMultiLineString mls)
                {
                    var segments = mls.Coordinates.Select(c => new TrackSegment(c.Select(c => new Point(Location.FromCoordinates(c).TransformInPlace(transform)))));
                    return new GeoPath(new Track(segments));
                }
                if (geometry is GeoJsonPolygon p)
                {
                    var segments = p.Coordinates.Select(c0 => new TrackSegment(c0.Select(c1 => new Point(Location.FromCoordinates(c1).TransformInPlace(transform)))));
                    return new GeoPath(new Track(segments));
                }
                if (geometry is GeoJsonMultiPolygon mpo)
                {
                    var tracks = mpo.Coordinates.Select(c0 => new Track( c0.Select(c1=>new TrackSegment(c1.Select(c2 => new Point(Location.FromCoordinates(c2).TransformInPlace(transform)))))));
                    return new GeoPath(tracks);
                }
            }
            return default(GeoPath);
        }
        public static Vector3? GetPivot(this IGeofencingShape shape)
        {
            if (shape.Anchor.HasValue)
            {
                return shape.Anchor.Value;
            }
            // remember that Shape Envelope are NOT transformed.
            var env = shape.Envelope;
            var pivot = env?.GetCenter(); ;
            if (pivot != null)
            {
                return new Vector3((float)pivot.Longitude, (float)pivot.Latitude, (float)(pivot.Altitude.HasValue ? pivot.Altitude.Value : 0));
            }
            return null;
        }

    }
}
