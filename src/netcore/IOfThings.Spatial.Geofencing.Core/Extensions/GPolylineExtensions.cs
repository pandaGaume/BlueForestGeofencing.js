using IOfThings.Spatial.Geography;
using IOfThings.Spatial.Text.GeoJson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace IOfThings.Spatial.Geofencing
{
    public static class GPolylineExtensions
    {
        public static IEnvelope BuildEnvelopeWithRadius(this IGPolyline shape)
        {
            if (shape.Geofence == null || shape.Geofence.Geometry == null) return null;
            var geometry = shape.Geofence.Geometry;
            if (!Radius.IsNullOrEmpty(shape.Radius))
            {
                EllipticSystem s = shape.Geofence.GeodeticSystem ?? EllipticSystem.WGS84;

                // doing all the calculation as radian save a lot of ressources.
                ILocation[] locAsRadian = null;

                if (geometry is GeoJsonMultiPoint mp)
                {
                    locAsRadian = mp.Positions.Select(p => new Location(p.Latitude * Ellipsoid.d2r, p.Longitude * Ellipsoid.d2r, p.Altitude)).ToArray();
                }
                else if (geometry is GeoJsonLineString ls)
                {
                    locAsRadian = ls.Positions.Select(p => new Location(p.Latitude * Ellipsoid.d2r, p.Longitude * Ellipsoid.d2r, p.Altitude)).ToArray();
                }

                if (locAsRadian != null)
                {
                    ILocation a = null, b = null;
                    double lat, lon;
                    Envelope envAsRadian = new Envelope();
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

        public static IEnumerable<ILocation> GetPolyline(this IGPolyline shape, Matrix4x4 t)
        {
            if (shape?.Geofence?.Geometry != null)
            {
                var geometry = shape.Geofence.Geometry;

                // accept feature as indirection
                if (geometry is GeoJsonFeature f)
                {
                    geometry = f.Geometry;
                }
                if (geometry is IGeoJsonMultiPoint p)
                {
                    var coordinates = p.Coordinates;
                    if (coordinates != null)
                    {
                        IEnumerable<ILocation> locations = coordinates.Select((c) => new Location(c[1], c[0], c.Length > 2 ? (float?)c[2] : null));
                        if (!t.IsIdentity)
                        {
                            locations = locations.Select((l) => l.TransformInPlace(t));
                        }
                        return locations;
                    }
                }
            }
            return Enumerable.Empty<ILocation>();
        }
    }
}
